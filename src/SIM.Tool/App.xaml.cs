#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml;
using SIM.Adapters.SqlServer;
using SIM.Instances;
using SIM.Pipelines;
using SIM.Pipelines.Processors;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Windows;
using SIM.Tool.Wizards;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Base.Annotations;
using Sitecore.Diagnostics.Logging;
using File = System.IO.File;

#endregion

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable CSharpWarnings::CS0162
namespace SIM.Tool
{
  using log4net.Config;

  public partial class App
  {
    #region Fields

    public static readonly int APP_DUPLICATE_EXIT_CODE = -8;
    public static readonly int APP_NO_MAIN_WINDOW = -44;
    public static readonly int APP_PIPELINES_ERROR = -22;
    private static readonly string AppLogsMessage = "The application will be suspended, look at the " + ApplicationManager.LogsFolder + " log file to find out what has happened";

    #endregion

    #region Methods

    #region Public methods

    public static string GetLicensePath()
    {
      return Path.Combine(ApplicationManager.DataFolder, "license.xml");
    }

    public static string GetRepositoryPath()
    {
      string path = Path.Combine(ApplicationManager.DataFolder, "Repository");
      FileSystem.FileSystem.Local.Directory.Ensure(path);
      return path;
    }

    #endregion

    #region Protected methods

    protected override void OnExit(ExitEventArgs e)
    {
      LifeManager.ReleaseSingleInstanceLock();
      base.OnExit(e);
    }

    protected override void OnStartup([CanBeNull] StartupEventArgs e)
    {
      base.OnStartup(e);

      // Ensure we are running only one instance of process
      if (!App.AcquireSingleInstanceLock())
      {
        try
        {
          // Change exit code to some uniqueue value to recognize reason of the app closing
          LifeManager.ShutdownApplication(APP_DUPLICATE_EXIT_CODE);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "An unhandled error occurred during shutting down");
        }

        return;
      }

      try
      {
        // If this is restart, wait until the master instance exists.
        LifeManager.WaitUntilOriginalInstanceExits(e.Args);

        // Capture UI sync context. It will allow to invoke delegates on UI thread in more elegant way (rather than use Dispatcher directly).
        LifeManager.UISynchronizationContext = SynchronizationContext.Current;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("An unhandled error occurred during LifeManager work", true, ex);
      }

      App.InitializeLogging();

      App.LogMainInfo();

      // Initializing pipelines from Pipelines.config and WizardPipelines.config files
      if (!App.InitializePipelines())
      {
        LifeManager.ShutdownApplication(APP_PIPELINES_ERROR);
        return;
      }

      // Application is closing when it doesn't have any window instance therefore it's 
      // required to create MainWindow before creating the initial configuration dialog
      var main = App.CreateMainWindow();
      if (main == null)
      {
        LifeManager.ShutdownApplication(APP_NO_MAIN_WINDOW);
        return;
      }

      // Initialize Profile Manager
      if (!App.InitializeProfileManager(main))
      {
        Log.Info("Application closes due to invalid configuration");

        // Since the main window instance was already created we need to "dispose" it by showing and closing.
        main.Width = 0;
        main.Height = 0;
        main.Show();
        main.Close();
        LifeManager.ShutdownApplication(2);
        return;
      }

      // Check if user accepted agreement
      var agreementAcceptedFilePath = Path.Combine(ApplicationManager.TempFolder, "agreement-accepted.txt");
      if (!File.Exists(agreementAcceptedFilePath))
      {
        WizardPipelineManager.Start("agreement", main, new ProcessorArgs(), false);
        if (!File.Exists(agreementAcceptedFilePath))
        {
          return;
        }
      }

      // Run updater
      App.RunUpdater();

      // Initializing plugins asynchronously 
      PluginManager.Initialize();

      // Clean up garbage
      App.DeleteTempFolders();

      // Show main window
      try
      {
        main.Initialize();
        WindowHelper.ShowDialog(main, null);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Main window caused unhandled exception", true, ex);
      }
    }

    #endregion

    #region Private methods

    private static void InitializeLogging()
    {
      Log.Initialize(new Log4NetLogProvider());
      XmlConfigurator.Configure(new FileInfo("Log.config"));
    }

    private static bool AcquireSingleInstanceLock()
    {
      try
      {
        return LifeManager.AcquireSingleInstanceLock();
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Error occurred during acquiring single instance lock", true, ex);

        return true;
      }
    }

    private static MainWindow CreateMainWindow()
    {
      try
      {
        return new MainWindow();
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("The main window thrown an exception during creation. " + AppLogsMessage, true, ex);
        return null;
      }
    }

    private static void DeleteTempFolders()
    {
      try
      {
        FileSystem.FileSystem.Local.Directory.DeleteTempFolders();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Deleting temp folders caused an exception");
      }
    }

    private static Profile DetectProfile()
    {
      try
      {
        InstanceManager.Initialize();
        var instances = InstanceManager.Instances;
        if (!instances.Any())
        {
          return null;
        }

        var database = instances.Select(x => Safe(() => x.AttachedDatabases.FirstOrDefault(y => y.Name.EqualsIgnoreCase("core")), x.ToString())).FirstOrDefault(x => x != null);
        var cstr = SqlServerManager.Instance.GetManagementConnectionString(database.ConnectionString).ToString();
        var instance = instances.FirstOrDefault();
        var root = instance.RootPath.EmptyToNull().With(x => Path.GetDirectoryName(x)) ?? "C:\\inetpub\\wwwroot";
        var rep = GetRepositoryPath();
        var lic = GetLicensePath();
        if (!FileSystem.FileSystem.Local.File.Exists(lic))
        {
          FileSystem.FileSystem.Local.File.Copy(instance.LicencePath, lic);
        }

        return new Profile
        {
          ConnectionString = cstr, 
          InstancesFolder = root, 
          LocalRepository = rep, 
          License = lic
        };
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during detecting profile defaults");

        return new Profile();
      }
    }

    private static bool InitializePipelines()
    {
      using (new ProfileSection("Re-initializing pipelines"))
      {
        try
        {
          var wizardPipelinesConfig = XmlDocumentEx.LoadFile(WizardPipelineManager.WizardpipelinesConfigFilePath);
          var pipelinesNode = wizardPipelinesConfig.SelectSingleNode("/configuration/pipelines") as XmlElement;
          Assert.IsNotNull(pipelinesNode, "pipelinesNode2");

          var pipelinesConfig = XmlDocumentEx.LoadFile(PipelineManager.PipelinesConfigFilePath);
          pipelinesConfig.Merge(XmlDocumentEx.LoadXml("<configuration>" + pipelinesNode.OuterXml + "</configuration>"));

          var resultPipelinesNode = pipelinesConfig.SelectSingleNode("configuration/pipelines") as XmlElement;
          Assert.IsNotNull(resultPipelinesNode, "Can't find pipelines configuration node");

          PipelineManager.Initialize(resultPipelinesNode);

          return true;
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Error during initializing pipelines", true, ex);

          return false;
        }
      }
    }

    private static bool InitializeProfileManager(Window mainWindow)
    {
      using (new ProfileSection("Initialize profile manager"))
      {
        ProfileSection.Argument("mainWindow", mainWindow);

        try
        {
          ProfileManager.Initialize();
          if (ProfileManager.IsValid)
          {
            return ProfileSection.Result(true);
          }

          // if current profile is not valid then we will show the legacy profile if it exists, or at least use invalid one
          WizardPipelineManager.Start("setup", mainWindow, null, false, null, ProfileManager.Profile ?? DetectProfile());
          if (ProfileManager.IsValid)
          {
            return ProfileSection.Result(true);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Profile manager failed during initialization. " + AppLogsMessage, true, ex);
        }

        return ProfileSection.Result(false);
      }
    }

    private static void LogMainInfo()
    {
      try
      {
        Log.Info("**********************************************************************");
        Log.Info("**********************************************************************");
        Log.Info("Sitecore Instance Manager started");
        Log.Info("Version: {0}",  ApplicationManager.AppVersion);
        Log.Info("Revision: {0}",  ApplicationManager.AppRevision);
        Log.Info("Label: {0}",  ApplicationManager.AppLabel);

        var nativeArgs = Environment.GetCommandLineArgs();
        var commandLineArgs = nativeArgs.Skip(1).ToArray();
        var argsToLog = commandLineArgs.Length > 0 ? string.Join("|", commandLineArgs) : "<NO ARGUMENTS>";

        Log.Info("Executable: {0}",  nativeArgs.FirstOrDefault() ?? string.Empty);
        Log.Info("Arguments: {0}",  argsToLog);
        Log.Info("Directory: {0}",  Environment.CurrentDirectory);
        Log.Info("**********************************************************************");
        Log.Info("**********************************************************************");
      }
      catch
      {
        Debug.WriteLine("Error during log main info");
      }
    }

    private static void RunUpdater()
    {
#if DEBUG
      return;
#endif
      if (!ApplicationManager.IsDebugging)
      {
        try
        {
          Log.Info("Running updater");
          const string updaterFileName = "Updater.exe";
          const string newUpdaterFileName = "Updater_new.exe";

          if (Process.GetProcessesByName(updaterFileName).Any())
          {
            return;
          }

          if (Process.GetProcessesByName(newUpdaterFileName).Any())
          {
            return;
          }

          if (File.Exists(newUpdaterFileName))
          {
            if (File.Exists(updaterFileName))
            {
              File.Delete(updaterFileName);
            }

            File.Move(newUpdaterFileName, updaterFileName);
          }

          const string updaterConfigFileName = "Updater.exe.config";
          const string newUpdaterConfigFileName = "Updater_new.exe.config";
          if (File.Exists(newUpdaterConfigFileName))
          {
            if (File.Exists(updaterConfigFileName))
            {
              File.Delete(updaterConfigFileName);
            }

            File.Move(newUpdaterConfigFileName, updaterConfigFileName);
          }

          if (!File.Exists(updaterConfigFileName))
          {
            File.WriteAllText(updaterConfigFileName, @"<?xml version=""1.0""?>
<configuration>
  <appSettings>
    <add key=""ProductTitle"" value=""Sitecore Instance Manager""/>
    <add key=""ProductFileName"" value=""SIM.Tool.exe""/>
    <add key=""UpdaterEnabled"" value=""yes""/>
    <add key=""IgnoreList"" value=""Updater.exe|Updater.exe.config|Updater.vshost.exe|Updater.log|SIM.Tool.vshost.exe|TaskDialog.dll""/>
    <add key=""ClearCacheFolders"" value=""%APPDATA%\Sitecore\Sitecore Instance Manager\Caches""/>
    <add key=""LatestVersionURL"" value=""http://dl.sitecore.net/updater/1.1/sim/latest-version.txt""/>
    <add key=""DownloadURL"" value=""http://dl.sitecore.net/updater/1.1/sim/download.txt""/>
    <add key=""MessageURL"" value=""http://dl.sitecore.net/updater/1.1/sim/message.txt""/>
  </appSettings>
</configuration>");
            Thread.Sleep(100);
          }

          if (!File.Exists(updaterFileName))
          {
            var updaterFilePath = ApplicationManager.GetEmbeddedApp("Updater.zip", "SIM.Tool", "updater.exe");
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(updaterFilePath)))
            {
              File.Copy(file, Path.GetFileName(file));
            }

            Thread.Sleep(100);
          }

          WindowHelper.RunApp(updaterFileName);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Updater caused unhandled exception", true, ex);
        }
      }
    }

    private static T Safe<T>([NotNull] Func<T> func, [NotNull] string label)
    {
      Assert.ArgumentNotNull(func, "func");
      Assert.ArgumentNotNull(label, "label");

      try
      {
        return func();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Failed to process {0}",  label);

        return default(T);
      }
    }

    #endregion

    #endregion
  }
}
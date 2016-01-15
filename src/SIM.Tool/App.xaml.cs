// ReSharper disable HeuristicUnreachableCode
// ReSharper disable CSharpWarnings::CS0162
namespace SIM.Tool
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Security.Principal;
  using System.ServiceProcess;
  using System.Threading;
  using System.Windows;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Core;
  using SIM.Core.Common;
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

  public partial class App
  {
    #region Fields

    public static readonly int APP_DUPLICATE_EXIT_CODE = -8;
    public static readonly int APP_NO_MAIN_WINDOW = -44;
    public static readonly int APP_NO_PERMISSIONS = -66;
    public static readonly int APP_NO_IIS = -88;
    public static readonly int APP_PIPELINES_ERROR = -22;
    private static readonly string AppLogsMessage = "The application will be suspended, look at the " + ApplicationManager.LogsFolder + " log file to find out what has happened";

    #endregion

    #region Properties

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

      if (CoreApp.IsFirstRun)
      {
        CacheManager.ClearAll();

        Directory.Delete(ApplicationManager.TempFolder, true);

        foreach (var filePath in Directory.GetFiles(".", "*-xml", SearchOption.AllDirectories))
        {
          if (filePath == null)
          {
            continue;
          }

          var newFilePath = filePath.Substring(0, filePath.Length - 4) + ".xml";
          if (File.Exists(newFilePath))
          {
            File.Delete(newFilePath);
          }

          File.Move(filePath, newFilePath);
        }
      }

      if (!App.CheckPermissions())
      {
        LifeManager.ShutdownApplication(APP_NO_PERMISSIONS);

        return;
      }

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

      CoreApp.InitializeLogging();

      CoreApp.LogMainInfo();

      if (!App.CheckIIS())
      {
        WindowHelper.ShowMessage("Cannot connect to IIS. Make sure it is installed and running.", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        LifeManager.ShutdownApplication(APP_NO_IIS);
        return;
      }

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

      // Initializing plugins asynchronously 
      PluginManager.Initialize();

      // Clean up garbage
      CoreApp.DeleteTempFolders();

      Analytics.Start();

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

      Analytics.Flush();
    }

    private static bool CheckIIS()
    {
      try
      {
        using (var sc = new ServiceController("W3SVC"))
        {
          Log.Info("IIS.Name: {0}", sc.DisplayName);
          Log.Info("IIS.Status: {0}", sc.Status);
          Log.Info("IIS.MachineName: {0}", sc.MachineName);
          Log.Info("IIS.ServiceName: {0}", sc.ServiceName);
          Log.Info("IIS.ServiceType: {0}", sc.ServiceType);

          return sc.Status.Equals(ServiceControllerStatus.Running);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during checking IIS state");

        return false;
      }
    }

    private static bool CheckPermissions()
    {
      if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
      {
        return true;
      }

      // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
      // app as administrator in a new process.
      var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase)
      {
        UseShellExecute = true,
        Verb = "runas"
      };

      // Start the new process
      try
      {
        try
        {
          Process.Start(processInfo);
        }
        catch (Win32Exception ex)
        {
          if (ex.NativeErrorCode != 1223)
          {
            throw;
          }

          Log.Info("User cancelled permissions elevation");
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "An unhandled exception was thrown");
      }

      return false;
    }

    #endregion

    #region Private methods

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

    private static Base.Profiles.Profile DetectProfile()
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

        return new Base.Profiles.Profile
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

        return new Base.Profiles.Profile();
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
        Log.Error(ex, "Failed to process {0}", label);

        return default(T);
      }
    }

    #endregion

    #endregion
  }
}
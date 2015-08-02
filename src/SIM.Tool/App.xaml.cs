// ReSharper disable HeuristicUnreachableCode
// ReSharper disable CSharpWarnings::CS0162

using System.Net;

namespace SIM.Tool
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using System.Security.Principal;
  using System.ServiceProcess;
  using System.Windows;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Core;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Windows;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Tool.Base.Wizards;
  using File = System.IO.File;

  public partial class App
  {
    #region Fields

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

    protected override void OnStartup([CanBeNull] StartupEventArgs e)
    {
      base.OnStartup(e);

      if (!App.EnsureSingleProcess(e.Args))
      {
        Environment.Exit(0);

        return;
      }

      if (CoreApp.HasBeenUpdated)
      {
        var ver = ApplicationManager.AppVersion;
        if (!string.IsNullOrEmpty(ver))
        {
          var exists = false;
          var wc = new WebClient();
          var url = "https://github.com/Sitecore/Sitecore-Instance-Manager/releases/tag/" + ver;
          try
          {
            wc.DownloadString(url);
            exists = true;
          }
          catch
          {
            Log.Warn("Tag was not found: {0}", url);
          }

          if (exists)
          {
            CoreApp.OpenInBrowser(url, true);
          }
        }
      }
      
      if (CoreApp.IsVeryFirstRun || CoreApp.HasBeenUpdated)
      {
        CacheManager.ClearAll();
        foreach (var dir in Directory.GetDirectories(ApplicationManager.TempFolder))
        {
          Directory.Delete(dir, true);
        }

        var ext = ".deploy.txt";
        foreach (var filePath in Directory.GetFiles(".", "*" + ext, SearchOption.AllDirectories))
        {
          if (filePath == null)
          {
            continue;
          }

          var newFilePath = filePath.Substring(0, filePath.Length - ext.Length);
          if (File.Exists(newFilePath))
          {
            File.Delete(newFilePath);
          }

          File.Move(filePath, newFilePath);
        }
      }

      if (!App.CheckPermissions())
      {
        Environment.Exit(0);

        return;
      }

      CoreApp.InitializeLogging();

      CoreApp.LogMainInfo();

      if (!App.CheckIIS())
      {
        WindowHelper.ShowMessage("Cannot connect to IIS. Make sure it is installed and running.", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        Environment.Exit(0);

        return;
      }

      // Initializing pipelines from Pipelines.config and WizardPipelines.config files
      if (!App.InitializePipelines())
      {
        Environment.Exit(0);

        return;
      }

      // Application is closing when it doesn't have any window instance therefore it's 
      // required to create MainWindow before creating the initial configuration dialog
      var main = App.CreateMainWindow();
      if (main == null)
      {
        Environment.Exit(0);

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

        Environment.Exit(0);

        return;
      }

      // Check if user accepted agreement
      var agreementAcceptedFilePath = Path.Combine(ApplicationManager.TempFolder, "agreement-accepted.txt");
      if (!File.Exists(agreementAcceptedFilePath))
      {
        WizardPipelineManager.Start("agreement", main, new ProcessorArgs(), false);
        if (!File.Exists(agreementAcceptedFilePath))
        {
          Environment.Exit(0);

          return;
        }
      }

      // Clean up garbage
      CoreApp.DeleteTempFolders();

      App.LoadIocResourcesForSolr();
      Analytics.Start();


      CoreApp.WriteLastRunVersion();

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


      CoreApp.Exit();

      Analytics.Flush();

      Environment.Exit(0);
    }

    private static void LoadIocResourcesForSolr()
    {

      if (!Directory.Exists("IOC_Containers"))
      {
        Log.Info("Copying IOC dlls", typeof(App));
        ApplicationManager.GetEmbeddedFile("IOC_Containers.zip", "SIM.Pipelines", "IOC_Containers");
      }
    }

    private static bool EnsureSingleProcess(string[] args)
    {
      var count = args.Length == 1 && args.Single() == "child" ? 2 : 1;
      var currentSessionId = Process.GetCurrentProcess().SessionId;
      var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase));

      return processes.Count(x => x.SessionId == currentSessionId && !x.HasExited && x.PrivateMemorySize64 > 5000000) <= count;
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
        Arguments = "child",
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
        var instances = InstanceManager.Instances.ToArray();
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
          var wizardPipelinesConfig = XmlDocumentEx.LoadXml(WizardPipelinesConfig.Contents);
          var pipelinesNode = wizardPipelinesConfig.SelectSingleNode("/configuration/pipelines") as XmlElement;
          Assert.IsNotNull(pipelinesNode, "pipelinesNode2");

          var pipelinesConfig = XmlDocumentEx.LoadXml(PipelinesConfig.Contents);
          pipelinesConfig.Merge(XmlDocumentEx.LoadXml(pipelinesNode.OuterXml));

          var resultPipelinesNode = pipelinesConfig.SelectSingleNode("/pipelines") as XmlElement;
          Assert.IsNotNull(resultPipelinesNode, "Can't find pipelines configuration node");

          PipelineManager.Initialize(resultPipelinesNode);

          WizardPipelineManager.Initialize(wizardPipelinesConfig.DocumentElement);

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

namespace SIM
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using Ionic.Zip;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;
  using System.Timers;
  using System.ServiceProcess;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using System.Collections.Generic;
  using Newtonsoft.Json;

  #region



  #endregion

  public static class ApplicationManager
  {
    #region Delegates

    public static event EventHandler<CancelEventArgs> AttemptToClose;

    #endregion

    #region Constants

    public const string AppDataRoot = @"%AppData%\Sitecore\Sitecore Instance Manager";
    public const string DefaultConfigurations = "Configurations";
    public const string DefaultPackages = "Packages";
    public const string DockerDesktopPath = @"C:\Program Files\Docker\Docker\Docker Desktop.exe";
    public const string DockerProcessName = "Docker Desktop";
    public const string DockerServiceName = "docker";
    public const string IisServiceName = "W3SVC";
    private const string DockerVersionOsKey = "Os";
    private const string DockerVersionWindowsValue = "windows";
    private const string DockerVersionLinuxValue = "linux";

    #endregion

    [NotNull]
    #region Fields
    public static string UnInstallParamsFolder { get; set; }

    [NotNull]
    public static string AppLabel { get; }

    [NotNull]
    public static string AppRevision { get; }

    [NotNull]
    public static string AppShortVersion { get; }

    [NotNull]
    public static string AppThreePartVersion { get; }

    [NotNull]
    public static string AppVersion { get; }

    [NotNull]
    public static string CachesFolder { get; }

    [NotNull]
    public static string ConfigurationPackagesFolder { get; }

    [NotNull]
    public static string DataFolder { get; }

    [NotNull]
    public static string FilePackagesFolder { get; }

    public static bool IsDebugging { get; }

    public static bool IsQa { get; }

    public static bool IsDev { get; }

    public static string ProcessName { get; }

    [NotNull]
    public static string LogsFolder { get; }
    
    [NotNull]
    public static string ProfilesFolder { get; }

    [NotNull]
    public static string TempFolder { get; }

    [NotNull]
    public static string UserManifestsFolder { get; }

    public static string AppsFolder { get; }

    public static bool IsIisRunning { get; set; }

    public static DockerStatus PreviousDockerStatus { get; set; }

    public static DockerStatus CurrentDockerStatus { get; set; }

    public static event PropertyChangedEventHandler IisStatusChanged;

    public static event PropertyChangedEventHandler DockerStatusChanged;

    private static readonly System.Timers.Timer Timer;

    private const double TimerInterval = 10000;

    #endregion

    public enum DockerStatus
    {
      RunningWithWindowsContainers,
      RunningWithLinuxContainers,
      Stopped
    }

    #region Constructors

    static ApplicationManager()
    {
      var processName = Process.GetCurrentProcess().ProcessName + ".exe";
      ProcessName = processName;
      IsDebugging = processName.ContainsIgnoreCase(".vshost.");
      IsQa = processName.ContainsIgnoreCase(".QA.");
      IsDev = processName.ContainsIgnoreCase(".DEV.");

      DataFolder = InitializeFolder(Environment.ExpandEnvironmentVariables(AppDataRoot + (IsQa ? "-QA" : "")));
      CachesFolder = InitializeDataFolder("Caches");
      FilePackagesFolder = InitializeDataFolder("Custom Packages");
      ConfigurationPackagesFolder = InitializeDataFolder("Custom Configurations");
      ProfilesFolder = InitializeDataFolder("Profiles");
      LogsFolder = InitializeDataFolder("Logs");
      AppsFolder = InitializeDataFolder("Apps");
      UserManifestsFolder = InitializeDataFolder("Manifests");
      AppRevision = GetRevision();
      AppVersion = GetVersion();
      AppShortVersion = GetShortVersion();
      AppThreePartVersion = GetThreePartVersion();
      AppLabel = GetLabel();
      UnInstallParamsFolder = InitializeDataFolder("UnInstallParams");
      TempFolder = InitializeDataFolder("Temp");
      IsIisRunning = GetIisStatus();
      CurrentDockerStatus = GetDockerStatus();
      Timer = new System.Timers.Timer(TimerInterval);
      Timer.Elapsed += Timer_Elapsed;
      Timer.Enabled = true;
    }

    #endregion

    #region Public methods

    [NotNull]
    public static string GetEmbeddedFile([NotNull] string assemblyName, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(assemblyName, nameof(assemblyName));
      Assert.ArgumentNotNull(fileName, nameof(fileName));

      var folder = Path.Combine(TempFolder, assemblyName);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      var filePath = Path.Combine(folder, fileName);
      if (File.Exists(filePath))
      {
        return filePath;
      }

      var assembly = Assembly.Load(assemblyName);
      Assert.IsNotNull(assembly, nameof(assembly));

      using (var stream = assembly.GetManifestResourceStream(assemblyName + @"." + fileName))
      {
        Assert.IsNotNull(stream, nameof(stream));

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          const int BufferSize = 2048;

          int len;
          var buffer = new byte[BufferSize];

          while ((len = stream.Read(buffer, 0, BufferSize)) > 0)
          {
            fileStream.Write(buffer, 0, len);
          }
        }

        Assert.IsTrue(File.Exists(filePath), $"The {filePath} file path doesn't exist after successful extracting {filePath} package into {folder} folder");

        return filePath;
      }
    }

    [NotNull]
    public static string GetEmbeddedFile([NotNull] string packageName, [NotNull] string assemblyName, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(packageName, nameof(packageName));
      Assert.ArgumentNotNull(assemblyName, nameof(assemblyName));
      Assert.ArgumentNotNull(fileName, nameof(fileName));

      var folder = Path.Combine(TempFolder, assemblyName, packageName);
      var filePath = Path.Combine(folder, fileName);
      if (File.Exists(filePath) || Directory.Exists(filePath))
      {
        return filePath;
      }

      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      var assembly = Assembly.Load(assemblyName);
      Assert.IsNotNull(assembly, nameof(assembly));

      using (var stream = assembly.GetManifestResourceStream(assemblyName + @"." + packageName))
      {
        Assert.IsNotNull(stream, nameof(stream));

        var tempFilePath = Path.GetTempFileName();
        try
        {
          using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
          {
            const int BufferSize = 2048;

            int len;
            var buffer = new byte[BufferSize];

            while ((len = stream.Read(buffer, 0, BufferSize)) > 0)
            {
              fileStream.Write(buffer, 0, len);
            }
          }

          using (var zip = new ZipFile(tempFilePath))
          {
            zip.ExtractAll(folder, ExtractExistingFileAction.OverwriteSilently);
          }

        Assert.IsTrue(File.Exists(filePath) || Directory.Exists(filePath), $"The {filePath} file path doesn't exist after successful extracting {tempFilePath} package into {folder} folder");

          return filePath;
        }
        finally
        {
          if (File.Exists(tempFilePath))
          {
            File.Delete(tempFilePath);
          }
        }
      }
    }

    public static void RaiseAttemptToClose(CancelEventArgs e)
    {
      EventHelper.RaiseEvent(AttemptToClose, typeof(ApplicationManager), e);
    }

    public static bool StartStopIis(bool start)
    {
      Timer.Stop();
      try
      {
        using (ServiceController serviceController = new ServiceController(IisServiceName))
        {
          if (start)
          {
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(TimerInterval));
          }
          else
          {
            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(TimerInterval));
          }
        }

        RunTimerElapsed(4000);
        Timer.Start();
        return true;
      }
      catch
      {
        Timer.Start();
        return false;
      }
    }

    public static bool StartStopDocker(bool start)
    {
      Timer.Stop();
      try
      {
        if (start)
        {
          if (File.Exists(DockerDesktopPath))
          {
            Process process = Process.Start(DockerDesktopPath);
            if (process != null)
            {
              RunTimerElapsed(10000);
              Timer.Start();
              return true;
            }
          }
        }
        else
        {
          ServiceController serviceController = GetDockerService();

          if (serviceController != null)
          {
            serviceController.Stop();

            foreach (Process process in Process.GetProcessesByName(DockerProcessName))
            {
              process.Kill();
            }

            RunTimerElapsed(4000);
            Timer.Start();
            return true;
          }
        }

        Timer.Start();
        return false;
      }
      catch
      {
        Timer.Start();
        return false;
      }
    }

    private static void RunTimerElapsed(int timeout)
    {
      Thread.Sleep(timeout);
      Timer_Elapsed(null, null);
    }

    #endregion

    #region Private methods

    private static string GetLabel()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyDescriptionAttribute);
      var descriptionAttribute = assembly.GetCustomAttributes(type, true);
      if (descriptionAttribute.Length == 0)
      {
        return string.Empty;
      }

      var label = descriptionAttribute[0] as AssemblyDescriptionAttribute;
      return label != null ? label.Description : string.Empty;
    }

    private static string GetRevision()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyInformationalVersionAttribute);
      var revisionAttribute = assembly.GetCustomAttributes(type, true);
      if (revisionAttribute.Length == 0)
      {
        return DateTime.Now.ToString("yyMMdd");
      }

      var revision = revisionAttribute[0] as AssemblyInformationalVersionAttribute;
      if (revision == null)
      {
        return String.Empty;
      }

      int revisionLength = revision.InformationalVersion.LastIndexOf(".", StringComparison.Ordinal);
      if (revisionLength != -1)
      {
        return revision.InformationalVersion.Substring(revisionLength + 1);
      }

      return String.Empty;
    }

    private static string GetShortVersion()
    {
      var version = GetVersion();
      if (string.IsNullOrEmpty(version))
      {
        return string.Empty;
      }

      return version.Substring(0, 3);
    }

    private static string GetThreePartVersion()
    {
      var version = GetVersion();
      if (string.IsNullOrEmpty(version))
      {
        return string.Empty;
      }

      int threePartVersionLength = version.LastIndexOf(".", StringComparison.Ordinal);
      if (threePartVersionLength != -1)
      {
        return version.Substring(0, threePartVersionLength);
      }

      return version;
    }

    private static string GetVersion()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof(AssemblyFileVersionAttribute);
      var versionAttribute = assembly.GetCustomAttributes(type, true);
      if (versionAttribute.Length == 0)
      {
        return string.Empty;
      }

      var version = versionAttribute[0] as AssemblyFileVersionAttribute;
      return version != null ? version.Version : string.Empty;
    }

    private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      bool iisStatus = GetIisStatus();
      if (IsIisRunning != iisStatus)
      {
        IsIisRunning = iisStatus;
        OnIisStatusChanged();
      }
      
      DockerStatus dockerStatus = GetDockerStatus();
      if (CurrentDockerStatus != dockerStatus)
      {
        PreviousDockerStatus = CurrentDockerStatus;
        CurrentDockerStatus = dockerStatus;
        OnDockerStatusChanged();
      }
    }

    private static bool GetIisStatus()
    {
      try
      {
        using (ServiceController serviceController = new ServiceController(IisServiceName))
        {
          if (serviceController.Status.Equals(ServiceControllerStatus.Running))
          {
            return true;
          }
          return false;
        }
      }
      catch
      {
        return false;
      }
    }

    private static DockerStatus GetDockerStatus()
    {
      try
      {
        string cmdOutput = string.Empty;

        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c docker version --format json")
        {
          CreateNoWindow = true,
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true
        };

        Process cmdProcess = Process.Start(processStartInfo);
        cmdProcess.OutputDataReceived += (sender, args) => { cmdOutput += args.Data; };
        cmdProcess.BeginOutputReadLine();
        cmdProcess.WaitForExit();

        DockerVersionInfo dockerVersionInfo = JsonConvert.DeserializeObject<DockerVersionInfo>(cmdOutput);

        if (dockerVersionInfo.Server == null || !dockerVersionInfo.Server.Keys.Contains(DockerVersionOsKey))
        {
          return DockerStatus.Stopped;
        }
        else if (dockerVersionInfo.Server[DockerVersionOsKey].ToString().Equals(DockerVersionLinuxValue, StringComparison.OrdinalIgnoreCase))
        {
          return DockerStatus.RunningWithLinuxContainers;
        }
        else if (dockerVersionInfo.Server[DockerVersionOsKey].ToString().Equals(DockerVersionWindowsValue, StringComparison.OrdinalIgnoreCase))
        {
          return DockerStatus.RunningWithWindowsContainers;
        }

        return DockerStatus.Stopped;
      }
      catch
      {
        return DockerStatus.Stopped;
      }
    }

    private struct DockerVersionInfo
    {
      public Dictionary<string, object> Client { get; set; }

      public Dictionary<string, object> Server { get; set; }
    }

    private static ServiceController GetDockerService()
    {
      return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == DockerServiceName);
    }

    private static void OnIisStatusChanged([CallerMemberName] string name = null)
    {
      IisStatusChanged?.Invoke(null, new PropertyChangedEventArgs(name));
    }

    private static void OnDockerStatusChanged([CallerMemberName] string name = null)
    {
      DockerStatusChanged?.Invoke(null, new PropertyChangedEventArgs(name));
    }

    #endregion

    #region Methods

    [NotNull]
    private static string InitializeDataFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      return InitializeFolder(Path.Combine(DataFolder, folder));
    }

    [NotNull]
    private static string InitializeFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      var path = Path.Combine(Environment.CurrentDirectory, folder);
      if (!FileSystem.FileSystem.Local.Directory.Exists(folder))
      {
        FileSystem.FileSystem.Local.Directory.CreateDirectory(folder);
      }

      return path;
    }

    #endregion
  }
}
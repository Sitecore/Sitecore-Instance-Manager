#region Usings

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

#endregion

namespace SIM.Base
{
  


  #region

  using System.ComponentModel;

  #endregion

  /// <summary>
  ///   The application manager.
  /// </summary>
  public static class ApplicationManager
  {
    #region Constants

    /// <summary>
    /// The stock file packages.
    /// </summary>
    public const string StockPlugins = "Plugins";
    public const string DefaultPackages = "Packages";
    public const string DefaultConfigurations = "Configurations";

    #endregion

    public static event EventHandler<CancelEventArgs> AttemptToClose;

    #region Fields
    
    private const string AppDataRoot = @"%AppData%\Sitecore\Sitecore Instance Manager";
    public static readonly string DataFolder = InitializeFolder(Environment.ExpandEnvironmentVariables(AppDataRoot));
    public static readonly string PluginsFolder = InitializeDataFolder("Plugins");
    public static readonly string CachesFolder = InitializeDataFolder("Caches");
    public static readonly string FilePackagesFolder = InitializeDataFolder("Custom Packages");
    public static readonly string ConfigurationPackagesFolder = InitializeDataFolder("Custom Configurations");
    public static readonly string ProfilesFolder = InitializeDataFolder("Profiles");
    public static readonly string LogsFolder = InitializeDataFolder("Logs");
    public static readonly string UserManifestsFolder = InitializeDataFolder("Manifests");
    public static readonly string AppRevision = GetRevision();
    public static readonly string AppVersion = GetVersion();
    public static readonly string AppShortVersion = GetShortVersion();
    public static readonly string AppLabel = GetLabel();
    public static readonly bool IsDebugging = Environment.GetCommandLineArgs()[0].ContainsIgnoreCase("vshost.exe");
    public static readonly string TempFolder = InitializeDataFolder("Temp");

    public static bool IsInternal
    {
      //todo:change this
      get { return AppLabel.Contains("."); }
    }

    private static string GetRevision()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof (AssemblyInformationalVersionAttribute);
      var revisionAttribute = assembly.GetCustomAttributes(type, true);
      if (revisionAttribute.Length == 0) return DateTime.Now.ToString("yyMMdd");
      var revision = (revisionAttribute[0]) as AssemblyInformationalVersionAttribute;
      var rev = "rev. ";
      return revision != null ? revision.InformationalVersion.Remove(0, (revision.InformationalVersion.IndexOf(rev, StringComparison.Ordinal) + rev.Length)) : string.Empty;
    }

    private static string GetVersion()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof (AssemblyFileVersionAttribute);
      var versionAttribute = assembly.GetCustomAttributes(type, true);
      if (versionAttribute.Length == 0) return string.Empty;
      var version = (versionAttribute[0]) as AssemblyFileVersionAttribute;
      return version != null ? version.Version : string.Empty;
    }

    private static string GetShortVersion()
    {
      var version = GetVersion();
      if (string.IsNullOrEmpty(version)) return string.Empty;
      return version.Substring(0, 3);
    }

    private static string GetLabel()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var type = typeof (AssemblyDescriptionAttribute);
      var descriptionAttribute = assembly.GetCustomAttributes(type, true);
      if (descriptionAttribute.Length == 0) return string.Empty;
      var label = (descriptionAttribute[0]) as AssemblyDescriptionAttribute;
      return label != null ? label.Description : string.Empty;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the data folder.
    /// </summary>
    /// <param name="folder">
    /// The folder. 
    /// </param>
    /// <returns>
    /// The data folder. 
    /// </returns>
    [NotNull]
    private static string InitializeDataFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, "folder");

      return InitializeFolder(Path.Combine(DataFolder, folder));
    }

    /// <summary>
    /// Initializes the folder.
    /// </summary>
    /// <param name="folder">
    /// The folder. 
    /// </param>
    /// <returns>
    /// The folder 
    /// </returns>
    [NotNull]
    private static string InitializeFolder([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, "folder");

      string path = Path.Combine(Environment.CurrentDirectory, folder);
      if (!FileSystem.Local.Directory.Exists(folder))
      {
        FileSystem.Local.Directory.CreateDirectory(folder);
      }

      return path;
    }

    #endregion

    public static void RaiseAttemptToClose(CancelEventArgs e)
    {
      EventHelper.RaiseEvent(AttemptToClose, typeof(ApplicationManager), e);
    }
  }
}
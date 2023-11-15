namespace SIM.Tool.Windows
{
  using JetBrains.Annotations;

  public static class WindowsSettings
  {
    #region Fields

    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloader8ParallelThreads = AdvancedSettings.Create("App/Downloader8/ParallelThreads", 1);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderExternalRepository = AdvancedSettings.Create("App/Downloader/ExternalRepository", string.Empty);
    
    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloaderParallelThreads = AdvancedSettings.Create("App/Downloader/ParallelThreads", 4);

    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloaderTotalTimeout = AdvancedSettings.Create("App/Downloader/TotalTimeoutHours", 24);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallDefaultCustomPackages = AdvancedSettings.Create("App/Install/Default/CustomPackages", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppInstallEverywhere = AdvancedSettings.Create("App/Install/Everywhere", false);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultFramework = AdvancedSettings.Create("App/Install/Default/FrameworkVersion", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultPoolMode = AdvancedSettings.Create("App/Install/Default/AppPoolMode", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProduct = AdvancedSettings.Create("App/Install/Default/ProductName", "Sitecore CMS");

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProductRevision = AdvancedSettings.Create("App/Install/Default/ProductRevision", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProductVersion = AdvancedSettings.Create("App/Install/Default/ProductVersion", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppInstanceSearchEnabled = AdvancedSettings.Create("App/InstanceSearch/Enabled", true);

    [NotNull]
    public static readonly AdvancedProperty<int> AppInstanceSearchTimeout = AdvancedSettings.Create("App/InstanceSearch/Timeout", 300);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppTelemetryEnabled = AdvancedSettings.Create("App/Telemetry/Enabled", true);

    [NotNull]
    public static readonly AdvancedProperty<string> AppToolsConfigEditor = AdvancedSettings.Create("App/Tools/ConfigEditor", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<int> AppUiMainWindowWidth = AdvancedSettings.Create("App/UI/MainWindowWidth", -1);

    [NotNull]
    public static readonly AdvancedProperty<string> AppThemeBackgroundPending = AdvancedSettings.Create("App/Theme/Background/Pending", "#e8e39a");

    [NotNull]
    public static readonly AdvancedProperty<string> AppThemeBackgroundError = AdvancedSettings.Create("App/Theme/Background/Error", "#e86f6d");

    [NotNull]
    public static readonly AdvancedProperty<string> AppThemeBackgroundWarning = AdvancedSettings.Create("App/Theme/Background/Warning", "#e8e39a");

    [NotNull]
    public static readonly AdvancedProperty<string> AppThemeBackgroundSuccess = AdvancedSettings.Create("App/Theme/Background/Success", "#ccffcc");

    [NotNull]
    public static readonly AdvancedProperty<string> AppThemeBackgroundDisabled = AdvancedSettings.Create("App/Theme/Background/Disabled", "#f2f2f2");

    #endregion
  }
}
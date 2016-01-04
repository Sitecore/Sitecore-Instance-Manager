namespace SIM.Tool.Windows
{
  using Sitecore.Diagnostics.Base.Annotations;

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
    public static readonly AdvancedProperty<string> AppDownloaderSdnPassword = AdvancedSettings.Create("App/Downloader/SDN/Password", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderSdnUserName = AdvancedSettings.Create("App/Downloader/SDN/UserName", string.Empty);

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
    public static readonly AdvancedProperty<string> AppToolsConfigEditor = AdvancedSettings.Create("App/Tools/ConfigEditor", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppUiMainWindowDoubleClick = AdvancedSettings.Create("App/UI/InstanceDoubleClick", @"SIM.Tool.Windows.MainWindowComponents.BrowseButton, SIM.Tool.Windows");

    [NotNull]
    public static readonly AdvancedProperty<int> AppUiMainWindowWidth = AdvancedSettings.Create("App/UI/MainWindowWidth", -1);

    [NotNull]
    public static readonly AdvancedProperty<string> AppNuGetDirectory = AdvancedSettings.Create("App/NuGet/Directory", "%PROGRAMDATA%\\Sitecore\\NuGet");

    #endregion
  }
}
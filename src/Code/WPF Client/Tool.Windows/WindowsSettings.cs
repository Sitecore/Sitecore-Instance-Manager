using System;
using SIM.Base;

namespace SIM.Tool.Windows
{
  public static class WindowsSettings
  {
    [NotNull]
    public readonly static AdvancedProperty<string> AppInstallDefaultCustomPackages = AdvancedSettings.Create("App/Install/Default/CustomPackages", String.Empty);

    [NotNull]
    public readonly static AdvancedProperty<int> AppInstanceSearchTimeout = AdvancedSettings.Create("App/InstanceSearch/Timeout", 300);

    [NotNull]
    public readonly static AdvancedProperty<bool> AppInstanceSearchEnabled = AdvancedSettings.Create("App/InstanceSearch/Enabled", true);

    [NotNull]
    public readonly static AdvancedProperty<int> AppUiMainWindowWidth = AdvancedSettings.Create("App/UI/MainWindowWidth", -1);

    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloaderParallelThreads = AdvancedSettings.Create("App/Downloader/ParallelThreads", 4);

    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloader8ParallelThreads = AdvancedSettings.Create("App/Downloader8/ParallelThreads", 1);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderSdnUserName = AdvancedSettings.Create("App/Downloader/SDN/UserName", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderSdnPassword = AdvancedSettings.Create("App/Downloader/SDN/Password", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<int> AppDownloaderTotalTimeout = AdvancedSettings.Create("App/Downloader/TotalTimeoutHours", 24);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderIndexUrl = AdvancedSettings.Create("App/Downloader/SDN/IndexUrl", "http://dl.sitecore.net/updater/1.1/sim/products.txt");

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloader8IndexUrl = AdvancedSettings.Create("App/Downloader8/SDN/IndexUrl", "http://dl.sitecore.net/updater/1.1/sim/products8.txt");

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProduct = AdvancedSettings.Create("App/Install/Default/ProductName", "Sitecore CMS");

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProductVersion = AdvancedSettings.Create("App/Install/Default/ProductVersion", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultProductRevision = AdvancedSettings.Create("App/Install/Default/ProductRevision", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultFramework = AdvancedSettings.Create("App/Install/Default/FrameworkVersion", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppInstallationDefaultPoolMode = AdvancedSettings.Create("App/Install/Default/AppPoolMode", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppDownloaderExternalRepository = AdvancedSettings.Create("App/Downloader/ExternalRepository", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppToolsConfigEditor = AdvancedSettings.Create("App/Tools/ConfigEditor", String.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppUiMainWindowDoubleClick = AdvancedSettings.Create("App/UI/InstanceDoubleClick", @"SIM.Tool.Windows.MainWindowComponents.BrowseButton, SIM.Tool.Windows");

    [NotNull]
    public readonly static AdvancedProperty<bool> AppInstallEverywhere = AdvancedSettings.Create("App/Install/Everywhere", false);
  }
}
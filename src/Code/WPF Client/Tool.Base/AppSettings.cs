using System;
using SIM.Base;

namespace SIM.Tool.Base
{
  public static class AppSettings
  {
    [NotNull]
    public readonly static AdvancedProperty<bool> AppSysIsSingleThreaded = AdvancedSettings.Create("App/Sys/IsSingleThreaded", false);

    [NotNull]
    public readonly static AdvancedProperty<bool> AppSysAllowMultipleInstances = AdvancedSettings.Create("App/Sys/AllowMultipleInstances", false);

    [NotNull]
    public readonly static AdvancedProperty<string> AppBrowsersFrontend = AdvancedSettings.Create("App/Browsers/Frontend", "explorer.exe");

    [NotNull]
    public static readonly AdvancedProperty<string> AppBrowsersBackend = AdvancedSettings.Create("App/Browsers/Backend", "explorer.exe");

    [NotNull]
    public readonly static AdvancedProperty<string> AppToolsVisualStudioVersion = AdvancedSettings.Create("App/Tools/VisualStudio/Version", "2012");

    [NotNull]
    public readonly static AdvancedProperty<string> AppToolsLogViewer = AdvancedSettings.Create("App/Tools/LogViewer", "logview.exe");

    [NotNull]
    public readonly static AdvancedProperty<bool> AppPreheatEnabled = AdvancedSettings.Create("App/Preheat/Enabled", false);

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminUserName = AdvancedSettings.Create("App/LoginAsAdmin/UserName", "sitecore\\admin");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminPageUrl = AdvancedSettings.Create("App/LoginAsAdmin/PageUrl", "/sitecore");
  }
}
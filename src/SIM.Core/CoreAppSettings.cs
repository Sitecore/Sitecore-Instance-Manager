namespace SIM.Core
{
  using JetBrains.Annotations;

  public static class CoreAppSettings
  {
    [NotNull]
    public static readonly AdvancedProperty<string> AppBrowsersBackend = AdvancedSettings.Create("App/Browsers/Backend", "explorer.exe");

    [NotNull]
    public static readonly AdvancedProperty<string> AppBrowsersFrontend = AdvancedSettings.Create("App/Browsers/Frontend", "explorer.exe");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminPageUrl = AdvancedSettings.Create("App/LoginAsAdmin/PageUrl", "/sitecore");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminUserName = AdvancedSettings.Create("App/LoginAsAdmin/UserName", "sitecore\\admin");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminNewPassword = AdvancedSettings.Create("App/LoginAsAdmin/NewPassword", "b");

    public static readonly AdvancedProperty<bool> CoreInstancesDetectEverywhere = AdvancedSettings.Create("Core/Instances/DetectEverywhere", false);
  }
}
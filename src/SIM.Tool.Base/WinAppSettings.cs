namespace SIM.Tool.Base
{
  using Sitecore.Diagnostics.Base.Annotations;

  public static class WinAppSettings
  {
    #region Fields

    [NotNull]
    public static readonly AdvancedProperty<bool> AppPreheatEnabled = AdvancedSettings.Create("App/Preheat/Enabled", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppUiHighDpiEnabled = AdvancedSettings.Create("App/UI/HighDPI/Enabled", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppSysIsSingleThreaded = AdvancedSettings.Create("App/Sys/IsSingleThreaded", false);

    [NotNull]
    public static readonly AdvancedProperty<string> AppToolsLogViewer = AdvancedSettings.Create("App/Tools/LogViewer", "logview.exe");
    
    #endregion
  }
}
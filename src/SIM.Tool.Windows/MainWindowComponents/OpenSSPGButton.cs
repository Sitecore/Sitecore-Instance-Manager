namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class OpenSSPGButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.RunApp("iexplore", "http://dl.sitecore.net/updater/clickonce/sspg/SSPG.application");
    }

    #endregion
  }
}
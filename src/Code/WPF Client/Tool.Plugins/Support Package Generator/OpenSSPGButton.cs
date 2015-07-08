namespace SIM.Tool.Plugins.SSPG
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public class OpenSSPGButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.RunApp("iexplore", "http://dl.sitecore.net/updater/clickonce/sspg/SSPG.application");
    }
  }
}

namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class OpenSSPGButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      CoreApp.RunApp("iexplore", "http://dl.sitecore.net/updater/sspg/SSPG.application");
    }

    #endregion
  }
}

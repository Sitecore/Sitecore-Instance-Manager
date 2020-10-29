using System.Windows;
using JetBrains.Annotations;
using SIM.Core;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
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

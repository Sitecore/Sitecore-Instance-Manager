using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenHostsButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      WindowHelper.ShowDialog(new HostsDialog(), mainWindow);
    }

    #endregion
  }
}
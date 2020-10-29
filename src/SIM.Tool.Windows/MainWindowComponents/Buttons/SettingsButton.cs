using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class SettingsButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      var result = WindowHelper.ShowDialog<SettingsDialog>(null, mainWindow);
      if (result != null)
      {
        MainWindowHelper.Initialize();
      }
    }

    #endregion
  }
}
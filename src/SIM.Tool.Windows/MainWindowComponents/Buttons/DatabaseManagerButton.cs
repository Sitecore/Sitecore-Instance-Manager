using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class DatabaseManagerButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      if (EnvironmentHelper.CheckSqlServer())
      {
        WindowHelper.ShowDialog(new DatabasesDialog(), mainWindow);
      }
    }

    #endregion
  }
}
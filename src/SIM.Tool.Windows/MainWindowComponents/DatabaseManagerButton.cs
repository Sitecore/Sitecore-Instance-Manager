namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using JetBrains.Annotations;

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
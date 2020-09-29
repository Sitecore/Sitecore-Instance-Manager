namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using JetBrains.Annotations;

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
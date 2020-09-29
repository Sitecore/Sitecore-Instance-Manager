namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using JetBrains.Annotations;

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
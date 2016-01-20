namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Windows.Dialogs;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class SettingsButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
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
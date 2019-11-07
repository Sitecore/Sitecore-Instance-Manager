namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class BackupButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      if (instance != null && (MainWindowHelper.IsSitecoreMember(instance) || MainWindowHelper.IsSitecore9(instance)))
      {
        return false;
      }

      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
    }

    #endregion
  }
}
namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class ExtraActionsButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return MainWindowHelper.IsEnabledOrVisibleButtonForSitecore9AndMember(instance);
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
    }

    #endregion
  }
}
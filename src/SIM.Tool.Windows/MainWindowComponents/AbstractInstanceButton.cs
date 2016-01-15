namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class InstanceOnlyButton : IMainWindowButton
  {
    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick([CanBeNull] Window mainWindow, Instance instance)
    {
      this.OnClick(instance);
    }

    protected abstract void OnClick([NotNull] Instance mainWindow);
  }
}
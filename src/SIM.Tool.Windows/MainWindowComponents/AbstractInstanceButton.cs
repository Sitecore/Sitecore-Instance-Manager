namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;

  public abstract class InstanceOnlyButton : IMainWindowButton
  {
    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return instance != null;
    }
    
    public virtual bool IsVisible([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick([CanBeNull] Window mainWindow, Instance instance)
    {
      OnClick(instance);
    }

    protected abstract void OnClick([NotNull] Instance mainWindow);
  }
}
namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class WindowOnlyButton : IMainWindowButton
  {
    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      this.OnClick(mainWindow);
    }

    protected abstract void OnClick([NotNull] Window mainWindow);
  }
}
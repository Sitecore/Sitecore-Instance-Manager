namespace SIM.Tool.Base.Plugins
{
  using System.Windows;
  using SIM.Base;
  using SIM.Instances;

  public interface IMainWindowButton
  {
    bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance);
  }
}

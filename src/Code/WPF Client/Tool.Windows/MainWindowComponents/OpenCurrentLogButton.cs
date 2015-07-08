namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Base;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  [UsedImplicitly]
  public class OpenCurrentLogButton : IMainWindowButton
  {
    [CanBeNull]
    private readonly string logFileType;

    public OpenCurrentLogButton()
    {
    }

    public OpenCurrentLogButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, "param");

      this.logFileType = param;
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      if (instance != null)
      {
        InstanceHelperEx.OpenCurrentLogFile(instance, mainWindow, this.logFileType);
      }
    }
  }
}

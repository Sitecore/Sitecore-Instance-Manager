namespace SIM.Tool.Plugins
{
  using System.Windows;
  using SIM.Base;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  [UsedImplicitly]
  public class InstallMongoDb: IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      WindowHelper.RunApp(@"Plugins\Install MongoDB\MongoDb.WindowsService.Installer.exe");
    }
  }
}
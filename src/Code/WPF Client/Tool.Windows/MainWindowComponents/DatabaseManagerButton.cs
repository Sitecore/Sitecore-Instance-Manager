using SIM.Tool.Base.Plugins;
using SIM.Tool.Windows.Dialogs;
using System.Windows;
using SIM.Instances;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class DatabaseManagerButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if(EnvironmentHelper.CheckSqlServer())
      {
        WindowHelper.ShowDialog(new DatabasesDialog(), mainWindow);
      }
    }
  }
}

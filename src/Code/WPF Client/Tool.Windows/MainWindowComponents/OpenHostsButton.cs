using SIM.Tool.Base.Plugins;
using System.Windows;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class OpenHostsButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.ShowDialog(new HostsDialog(), mainWindow);    
    }
  }
}

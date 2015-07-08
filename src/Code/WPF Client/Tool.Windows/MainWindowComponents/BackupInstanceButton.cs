using System.Windows;
using SIM.Instances;
using SIM.Pipelines.Backup;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class BackupInstanceButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("backup", mainWindow, new BackupArgs(instance), null, () => MainWindowHelper.MakeInstanceSelected(id), instance);
      }
    }
  }
}

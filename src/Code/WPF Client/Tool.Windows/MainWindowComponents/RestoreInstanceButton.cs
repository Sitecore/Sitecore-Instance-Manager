using System.Windows;
using SIM.Instances;
using SIM.Pipelines.Restore;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class RestoreInstanceButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var args = new RestoreArgs(instance);
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("restore", mainWindow, args, null, () => MainWindowHelper.MakeInstanceSelected(id), instance);
      }
    }
  }
}

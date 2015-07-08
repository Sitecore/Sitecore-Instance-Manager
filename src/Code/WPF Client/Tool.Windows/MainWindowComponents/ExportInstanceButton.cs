using System.Windows;
using SIM.Instances;
using SIM.Pipelines.Export;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class ExportInstanceButton : IMainWindowButton
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
        WizardPipelineManager.Start("export", mainWindow, new ExportArgs(instance, false, true, true, true, false, false, false, false, false), null, () => MainWindowHelper.MakeInstanceSelected(id), instance, string.Empty);
      }
    }
  }
}

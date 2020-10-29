using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class InstallModulesButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installmodules", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallModulesWizardArgs(instance));
      }
    }

    #endregion
  }
}
namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class InstallModulesButton : InstanceOnlyButton
  {
    #region Public methods

    public void OnClick(Window mainWindow, Instance instance)
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
namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.Export;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Export;

  [UsedImplicitly]
  public class ExportInstanceButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("export", mainWindow, new ExportArgs(instance, false, true, true, true, false, false, false, false, false), null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new ExportWizardArgs(instance, string.Empty));
      }
    }

    #endregion
  }
}
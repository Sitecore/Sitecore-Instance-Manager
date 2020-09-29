namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.Backup;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Backup;
  using SIM.Tool.Base.Profiles;

  [UsedImplicitly]
  public class BackupInstanceButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("backup", mainWindow, new BackupArgs(instance, ProfileManager.GetConnectionString()), null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new BackupSettingsWizardArgs(instance));
      }
    }

    #endregion
  }
}
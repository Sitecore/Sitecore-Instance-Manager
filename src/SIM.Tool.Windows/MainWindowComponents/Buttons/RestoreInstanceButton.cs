namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines.Restore;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using System.Data.SqlClient;
  using SIM.IO.Real;

  [UsedImplicitly]
  public class RestoreInstanceButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var args = new RestoreArgs(instance, new SqlConnectionStringBuilder(Profile.Read(new RealFileSystem()).ConnectionString));
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("restore", mainWindow, args, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new RestoreWizardArgs(instance));
      }
    }

    #endregion
  }
}
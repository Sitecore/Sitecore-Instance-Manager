using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Pipelines.Backup;
using SIM.Tool.Base;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Backup;
using SIM.Tool.Windows.UserControls.SitecoreAuthentication;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class BackupInstance9AndLaterButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        int.TryParse(instance.Product.ShortVersion, out int sitecoreVersion);

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("backup9AndLater", mainWindow, new BackupArgs(instance, ProfileManager.GetConnectionString()),
          null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new BackupWizard9Args(instance));
      }
    }

    #endregion
  }
}
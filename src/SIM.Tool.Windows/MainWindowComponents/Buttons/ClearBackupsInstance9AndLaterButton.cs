using System;
using System.Data.SqlClient;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using JetBrains.Annotations;
using SIM.Core.Common;
using SIM.Instances;
using SIM.IO.Real;
using SIM.Pipelines.Restore;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
//using SIM.Pipelines.ClearBackups;
using SIM.Tool.Base.Profiles;
using SIM.Pipelines.Backup;
using SIM.Tool.Windows.UserControls.Backup;
using Profile = SIM.Core.Common.Profile;
using SIM.Pipelines.ClearBackups;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class ClearBackupsInstance9AndLaterButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var args = new ClearBackupsArgs(instance);
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("clearbackups", mainWindow, args, null,
          ignore => MainWindowHelper.MakeInstanceSelected(id),
          () => new RemoveBackupsWizardArgs(instance));
      }
    }
    #endregion
  }
}
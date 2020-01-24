﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines.Backup;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Backup;
  using SIM.Tool.Base.Profiles;

  [UsedImplicitly]
  public class BackupInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
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
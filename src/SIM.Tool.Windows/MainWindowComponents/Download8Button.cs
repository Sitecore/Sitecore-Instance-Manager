﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Download8;

  [UsedImplicitly]
  public class Download8Button : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (FileSystem.FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download8", mainWindow, null, null, ignore => MainWindowHelper.RefreshInstaller(), () => new DownloadWizardArgs(WindowsSettings.AppDownloaderSdnUserName.Value, WindowsSettings.AppDownloaderSdnPassword.Value));
      }
    }

    #endregion
  }
}
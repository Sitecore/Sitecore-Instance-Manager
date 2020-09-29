namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Instances;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Download8;

  [UsedImplicitly]
  public class Download8Button : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      if (FileSystem.FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download8", mainWindow, null, null, ignore => MainWindowHelper.RefreshInstaller(), () => new DownloadWizardArgs(WindowsSettings.AppDownloaderSdnUserName.Value, WindowsSettings.AppDownloaderSdnPassword.Value));
      }
    }

    #endregion
  }
}
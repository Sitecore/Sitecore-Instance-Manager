namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class Download8Button : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("Download8");

      if (FileSystem.FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download8", mainWindow, null, null, MainWindowHelper.RefreshInstaller, WindowsSettings.AppDownloaderSdnUserName.Value, WindowsSettings.AppDownloaderSdnPassword.Value);
      }
    }

    #endregion
  }
}
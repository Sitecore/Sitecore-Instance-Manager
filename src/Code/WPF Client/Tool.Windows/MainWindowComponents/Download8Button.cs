namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Base;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Wizards;

  public class Download8Button : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download8", mainWindow, null, null, MainWindowHelper.RefreshInstaller, WindowsSettings.AppDownloaderSdnUserName.Value, WindowsSettings.AppDownloaderSdnPassword.Value);
      }
    }
  }
}
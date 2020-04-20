using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Download8;

namespace SIM.Tool.Windows.MainWindowComponents
{
  [UsedImplicitly]
  public class DownloadButton : IMainWindowButton
  {
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
  }
}
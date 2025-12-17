using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Download;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class DownloadButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      // Temporary solution for https://github.com/Sitecore/Sitecore-Instance-Manager/issues/801
      SIM.Tool.Base.WindowHelper.ShowMessage($"The functionality is currently unavailable. Please download a Sitecore package manually from https://developers.sitecore.com and put it to the '{ProfileManager.Profile.LocalRepository}' folder.",
        messageBoxImage: MessageBoxImage.Warning,
        messageBoxButton: MessageBoxButton.OK);
      return;

      if (FileSystem.FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download", mainWindow, null, null, ignore => MainWindowHelper.RefreshInstaller(), () => new DownloadWizardArgs());
      }
    }

    #endregion
  }
}
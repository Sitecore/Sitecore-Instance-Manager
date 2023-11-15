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
      if (FileSystem.FileSystem.Local.Directory.Exists(ProfileManager.Profile.LocalRepository))
      {
        WizardPipelineManager.Start("download", mainWindow, null, null, ignore => MainWindowHelper.RefreshInstaller(), () => new DownloadWizardArgs());
      }
    }

    #endregion
  }
}
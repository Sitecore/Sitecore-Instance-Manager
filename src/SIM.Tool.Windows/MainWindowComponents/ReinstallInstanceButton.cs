namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ReinstallInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("Reinstall");

      if (instance != null)
      {
        if (!MainWindowHelper.IsInstallerReady())
        {
          WindowHelper.ShowMessage(@"The installer isn't ready - check the Settings window", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
          return;
        }

        string license = ProfileManager.Profile.License;
        Assert.IsNotNull(license, @"The license file isn't set in the Settings window");
        FileSystem.FileSystem.Local.File.AssertExists(license, "The {0} file is missing".FormatWith(license));

        MainWindowHelper.ReinstallInstance(instance, mainWindow, license, ProfileManager.GetConnectionString());
      }
    }

    #endregion
  }
}
﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class ReinstallInstanceButton : IMainWindowButton
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
        if (!MainWindowHelper.IsInstallerReady())
        {
          WindowHelper.ShowMessage(@"The installer isn't ready - check the Settings window", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
          return;
        }

        var license = ProfileManager.Profile.License;
        Assert.IsNotNull(license, @"The license file isn't set in the Settings window");
        FileSystem.FileSystem.Local.File.AssertExists(license, "The {0} file is missing".FormatWith(license));
        if (int.Parse(instance.Product.ShortVersion) < 90)
        {
          MainWindowHelper.ReinstallInstance(instance, mainWindow, license, ProfileManager.GetConnectionString());
        }
        else
        {
          MainWindowHelper.Reinstall9Instance(instance, mainWindow, license, ProfileManager.GetConnectionString());
        }
      }
    }

    #endregion
  }
}
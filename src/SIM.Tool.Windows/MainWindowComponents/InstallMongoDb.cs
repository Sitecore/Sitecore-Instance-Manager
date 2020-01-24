﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class InstallMongoDbButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      CoreApp.RunApp(ApplicationManager.GetEmbeddedFile(@"MongoDb.WindowsService.Installer.zip", "SIM.Tool.Windows", @"MongoDb.WindowsService.Installer.exe"));
    }

    #endregion
  }
}
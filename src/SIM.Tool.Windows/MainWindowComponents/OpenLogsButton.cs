﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class OpenLogsButton : AbstractDownloadAndRunButton, IMainWindowButton
  {
    protected override string BaseUrl
    {
      get
      {
        return "http://dl.sitecore.net/updater/1.1/scla/";
      }
    }

    protected override string AppName
    {
      get
      {
        return "Log Analyzer";
      }
    }

    protected override string ExecutableName
    {
      get
      {
        return "SitecoreLogAnalyzer.exe";
      }
    }

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return MainWindowHelper.IsEnabledOrVisibleButtonForSitecoreMember(instance);
    }

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var dataFolderPath = instance.DataFolderPath;
        FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));

        var logs = Path.Combine(dataFolderPath, "logs");
        RunApp(mainWindow, logs);

        return;
      }

      RunApp(mainWindow);
    }

    #endregion
  }
}
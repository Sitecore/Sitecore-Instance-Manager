using System.IO;
using System.Windows;
using JetBrains.Annotations;
using SIM.Extensions;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Windows.LogFileFolder;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenLogsButton : AbstractDownloadAndRunButton
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

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string logs = LogFileFolderFactory.GetResolver(instance).GetLogFolder() ?? LogFileFolderFactory.GetDefaultResolver(instance).GetLogFolder();

        if (string.IsNullOrEmpty(logs))
        {
          WindowHelper.ShowMessage($"Unable to find the logs folder under '{instance.WebRootPath}'.", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
          RunApp(mainWindow, logs);
        }

        return;
      }

      RunApp(mainWindow);
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null && instance.Type == Instance.InstanceType.SitecoreContainer)
      {
        return false;
      }
      return base.IsVisible(mainWindow, instance);
    }

    #endregion

    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
    }

    #endregion
  }
}
using System.IO;
using System.Windows;
using JetBrains.Annotations;
using SIM.Extensions;
using SIM.Instances;

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
        var dataFolderPath = instance.DataFolderPath;
        FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));

        var logs = Path.Combine(dataFolderPath, "logs");
        RunApp(mainWindow, logs);

        return;
      }

      RunApp(mainWindow);
    }

    #endregion

    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
    }

    #endregion
  }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Plugins.LogAnalyzer
{
  public class OpenLogsButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      const string appFileName = "SitecoreLogAnalyzer.exe";
      const string configFileName = "SitecoreLogAnalyzer.config";
      const string directoryName = "Plugins\\Log Analyzer";
      string appFilePath = Environment.ExpandEnvironmentVariables(Path.Combine(directoryName, appFileName));
      string configFilePath = Environment.ExpandEnvironmentVariables(Path.Combine(directoryName, configFileName));
      if (!FileSystem.Local.File.Exists(appFilePath))
      {
        var hasLocally = WindowHelper.ShowMessage(@"This is your first usage of Log Analyzer plugin so little setup is required. 

Do you have Sitecore Log Analyzer installed locally?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (hasLocally != MessageBoxResult.Yes)
        {
          WindowHelper.OpenInBrowser(@"http://marketplace.sitecore.net/en/Modules/Sitecore_Log_Analyzer.aspx", true);
          WindowHelper.ShowMessage(
            @"OK, then you need to download it – the appropriate page was opened for you. Extract the downloaded archive to some location, click OK in this dialog and choose the SitecoreLogAnalyzer.exe file in the dialog that will be opened afterwards.");
        }

        var openDialog = new OpenFileDialog
          {
            CheckFileExists = true,
            Filter = "SitecoreLogAnalyzer.exe|SitecoreLogAnalyzer.exe",
            Title = "Choose SCLA executable"
          };

        var result = openDialog.ShowDialog(mainWindow);
        if (result != true)
        {
          return;
        }

        var openFilePath = openDialog.FileName;
        if (FileSystem.Local.File.Exists(openFilePath))
        {
          var openFolderPath = Path.GetDirectoryName(openFilePath);
          var openConfigFilePath = Path.Combine(openFolderPath, configFileName);
          FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(appFilePath));
          FileSystem.Local.File.Copy(openFilePath, appFilePath);
          FileSystem.Local.File.Copy(openConfigFilePath, configFilePath);
        }

        if (!FileSystem.Local.File.Exists(appFilePath))
        {
          return;
        }
      }
      
      if (instance != null)
      {
        string dataFolderPath = instance.DataFolderPath;
        FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));
        var logs = Path.Combine(dataFolderPath, "logs");
        WindowHelper.RunApp(appFilePath, logs);
        return;
      }

      WindowHelper.RunApp(appFilePath);
    }
  }
}

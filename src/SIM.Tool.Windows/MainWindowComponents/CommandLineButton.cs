namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;

  [UsedImplicitly]
  public class CommandLineButton : AbstractDownloadAndRunButton, IMainWindowButton
  {
    protected override string BaseUrl
    {
      get
      {
        var qaFolder = ApplicationManager.IsQA
          ? "qa/" 
          : string.Empty;

        return "http://dl.sitecore.net/updater/1.1/simcmd/" + qaFolder;
      }
    }

    protected override string AppName
    {
      get
      {
        return "SIMCMD";
      }
    }

    protected override string ExecutableName
    {
      get
      {
        return "sim.exe";
      }
    }

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("OpenCommandLine");

      if (instance != null)
      {
        string dataFolderPath = instance.DataFolderPath;
        FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));

        var logs = Path.Combine(dataFolderPath, "logs");
        RunApp(mainWindow, logs);

        return;
      }

      RunApp(mainWindow);
    }

    #endregion

    protected override void RunApp(string path, string param)
    {
      var start = new ProcessStartInfo("cmd.exe")
      {
        WorkingDirectory = AppFolder,
        Arguments = "/K echo %cd%^>sim & sim", // exectute two commands: print fake cmd output C:\somefolder>sim (bacause it is absent), and then do run sim;
        UseShellExecute = true
      };

      CoreApp.RunApp(start);
    }
  }
}

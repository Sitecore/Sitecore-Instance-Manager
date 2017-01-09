namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Core.Common;
  using SIM.Extensions;
  using SIM.Instances;

  [UsedImplicitly]
  public class CommandLineButton : AbstractDownloadAndRunButton
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

    public override void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("OpenCommandLine");

      base.OnClick(mainWindow, instance);
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

namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class CommandLineButton : WindowOnlyButton
  {
    protected override void OnClick([CanBeNull] Window mainWindow)
    {
      var appFilePath = ApplicationManager.GetEmbeddedFile("SIM.Tool.Windows", "SIM.exe");
      var targetDir = Path.GetDirectoryName(appFilePath);
      var sourceDir = Path.GetFullPath(".");
      foreach (var sourceFilePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
      {
        if (sourceFilePath.EndsWith("sim.exe", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        var targetFilePath = Path.Combine(targetDir, sourceFilePath.Substring(sourceDir.Length + 1));

        var dir = Path.GetDirectoryName(targetFilePath);
        if (!Directory.Exists(dir))
        {
          Directory.CreateDirectory(dir);
        }

        File.Copy(sourceFilePath, targetFilePath, true);
      }

      var start = new ProcessStartInfo("cmd.exe")
      {
        WorkingDirectory = targetDir,
        UseShellExecute = true
      };

      WindowHelper.RunApp(start);
    }
  }
}

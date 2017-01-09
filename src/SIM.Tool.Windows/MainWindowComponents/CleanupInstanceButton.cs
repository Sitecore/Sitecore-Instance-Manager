namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Linq;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public class CleanupInstanceButton : IMainWindowButton
  {
    [UsedImplicitly]
    public CleanupInstanceButton()
    {
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance.State != InstanceState.Stopped)
      {
        WindowHelper.ShowMessage("Stop the instance first");

        return;
      }

      WindowHelper.LongRunningTask(() => DoWork(instance), "Cleaning up", mainWindow);
    }

    private void DoWork(Instance instance)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));

      while (instance.ProcessIds.Any())
      {
        if (instance.State != InstanceState.Stopped)
        {
          MessageBox.Show("Stop the instance first");
        }
      }

      var tempFolder = instance.TempFolderPath;
      if (!string.IsNullOrEmpty(tempFolder) && Directory.Exists(tempFolder))
      {
        foreach (var dir in Directory.GetDirectories(tempFolder))
        {
          Directory.Delete(dir, true);
        }

        foreach (var file in Directory.GetFiles(tempFolder))
        {
          if (file.EndsWith("dictionary.dat"))
          {
            continue;
          }

          File.Delete(file);
        }
      }

      var folders = new[]
      {
        instance.LogsFolderPath,
        Path.Combine(instance.WebRootPath, "App_Data/MediaCache"),
        Path.Combine(instance.DataFolderPath, "ViewState"),
        Path.Combine(instance.DataFolderPath, "Diagnostics"),
        Path.Combine(instance.DataFolderPath, "Submit Queue"),
      };

      foreach (var folder in folders)
      {
        if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
        {
          continue;
        }
        Directory.Delete(folder, true);
        Directory.CreateDirectory(folder);
      }
    }
  }
}

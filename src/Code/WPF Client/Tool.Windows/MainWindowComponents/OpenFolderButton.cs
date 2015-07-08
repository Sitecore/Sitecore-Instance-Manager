using System;
using System.Windows;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class OpenFolderButton : IMainWindowButton
  {
    private readonly string folder;

    public OpenFolderButton(string folder)
    {
      Assert.IsNotNullOrEmpty(folder, "folder");

      this.folder = folder;
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null || !RequiresInstance(this.folder);
    }

    private bool RequiresInstance(string path)
    {
      return path.Contains("$");
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      var path = ExpandPath(instance).Replace("/", "\\");
      if (!FileSystem.Local.Directory.Exists(path))
      {
        var answer = WindowHelper.ShowMessage("The folder does not exist. Would you create it?\n\n" + path, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
        if (answer != MessageBoxResult.OK)
        {
          return;
        }

        FileSystem.Local.Directory.CreateDirectory(path);
      }

      MainWindowHelper.OpenFolder(path);
    }

    private string ExpandPath(Instance instance)
    {
      var path = this.folder;
      if (!string.IsNullOrEmpty(path) && !RequiresInstance(path))
      {
        return Environment.ExpandEnvironmentVariables(path);
      }

      path = path
        .Replace("$(root)", () => instance.RootPath)
        .Replace("$(website)", () => instance.WebRootPath)
        .Replace("$(data)", () => instance.DataFolderPath)
        .Replace("$(packages)", () => instance.PackagesFolderPath)
        .Replace("$(indexes)", () => instance.IndexesFolderPath)
        .Replace("$(serialization)", () => instance.SerializationFolderPath);
      
      if (RequiresInstance(path))
      {
        throw new InvalidOperationException("The {0} pattern is not supported".FormatWith(path));
      }

      return path;
    }
  }
}

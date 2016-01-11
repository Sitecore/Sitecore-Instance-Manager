namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class OpenFolderButton : IMainWindowButton
  {
    #region Fields

    private readonly string folder;

    #endregion

    #region Constructors

    public OpenFolderButton(string folder)
    {
      Assert.IsNotNullOrEmpty(folder, "folder");

      this.folder = folder;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null || !this.RequiresInstance(this.folder);
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("OpenFolder");

      var path = this.ExpandPath(instance).Replace("/", "\\");
      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        var answer = WindowHelper.ShowMessage("The folder does not exist. Would you create it?\n\n" + path, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
        if (answer != MessageBoxResult.OK)
        {
          return;
        }

        FileSystem.FileSystem.Local.Directory.CreateDirectory(path);
      }

      MainWindowHelper.OpenFolder(path);
    }

    #endregion

    #region Private methods

    private string ExpandPath(Instance instance)
    {
      var path = this.folder;
      if (!string.IsNullOrEmpty(path) && !this.RequiresInstance(path))
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

      if (this.RequiresInstance(path))
      {
        throw new InvalidOperationException("The {0} pattern is not supported".FormatWith(path));
      }

      return path;
    }

    private bool RequiresInstance(string path)
    {
      return path.Contains("$");
    }

    #endregion
  }
}
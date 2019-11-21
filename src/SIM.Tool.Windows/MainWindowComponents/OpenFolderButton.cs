namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class OpenFolderButton : IMainWindowButton
  {
    #region Fields

    private string Folder { get; }

    #endregion

    #region Constructors

    public OpenFolderButton(string folder)
    {
      Assert.IsNotNullOrEmpty(folder, nameof(folder));

      Folder = folder;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null || !RequiresInstance(Folder);
    }

    public virtual bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      var path = ExpandPath(instance).Replace("/", "\\");
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
      var path = Folder;
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

    private bool RequiresInstance(string path)
    {
      return path.Contains("$");
    }

    #endregion
  }
}
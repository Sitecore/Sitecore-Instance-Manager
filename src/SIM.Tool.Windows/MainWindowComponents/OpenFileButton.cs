namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Extensions;

  [UsedImplicitly]
  public class OpenFileButton : IMainWindowButton
  {
    #region Fields

    protected string FilePath { get; }

    #endregion

    #region Constructors

    public OpenFileButton(string param)
    {
      FilePath = param;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return MainWindowHelper.IsEnabledOrVisibleButtonForSitecoreMember(instance);
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("OpenFile");

      if (instance != null)
      {
        var filePath = FilePath.StartsWith("/") ? Path.Combine(instance.WebRootPath, FilePath.Substring(1)) : FilePath;
        FileSystem.FileSystem.Local.File.AssertExists(filePath, "The {0} file of the {1} instance doesn't exist".FormatWith(filePath, instance.Name));

        var editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          CoreApp.RunApp(editor, filePath);
        }
        else
        {
          CoreApp.OpenFile(filePath);
        }
      }
    }

    #endregion
  }
}
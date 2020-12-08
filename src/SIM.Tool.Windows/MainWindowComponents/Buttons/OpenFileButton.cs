using System.IO;
using System.Windows;
using JetBrains.Annotations;
using SIM.Core;
using SIM.Extensions;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenFileButton : InstanceOnlyButton
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

    public override void OnClick(Window mainWindow, Instance instance)
    {
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
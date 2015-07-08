using System.IO;
using System.Windows;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class OpenFileButton : IMainWindowButton
  {
    protected readonly string FilePath;

    public OpenFileButton(string param)
    {
      this.FilePath = param;
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string filePath = this.FilePath.StartsWith("/") ? Path.Combine(instance.WebRootPath, this.FilePath.Substring(1)) : this.FilePath;
        FileSystem.Local.File.AssertExists(filePath, "The {0} file of the {1} instance doesn't exist".FormatWith(filePath, instance.Name));

        string editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          WindowHelper.RunApp(editor, filePath);
        }
        else
        {
          WindowHelper.OpenFile(filePath);
        }
      }
    }
  }
}
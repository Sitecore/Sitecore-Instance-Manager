using System.Windows;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class OpenWebConfigButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string webConfigPath = WebConfig.GetWebConfigPath(instance.WebRootPath);
        FileSystem.Local.File.AssertExists(webConfigPath, "The web.config file ({0}) of the {1} instance doesn't exist".FormatWith(webConfigPath, instance.Name));
        string editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          WindowHelper.RunApp(editor, webConfigPath);
        }
        else
        {
          WindowHelper.OpenFile(webConfigPath);
        }
      }
    }
  }
}

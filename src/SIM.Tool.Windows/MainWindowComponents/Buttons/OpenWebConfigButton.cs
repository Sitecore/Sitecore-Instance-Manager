using System.Windows;
using JetBrains.Annotations;
using SIM.Adapters.WebServer;
using SIM.Core;
using SIM.Extensions;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenWebConfigButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var webConfigPath = WebConfig.GetWebConfigPath(instance.WebRootPath);
        FileSystem.FileSystem.Local.File.AssertExists(webConfigPath, "The web.config file ({0}) of the {1} instance doesn't exist".FormatWith(webConfigPath, instance.Name));
        var editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          CoreApp.RunApp(editor, webConfigPath);
        }
        else
        {
          CoreApp.OpenFile(webConfigPath);
        }
      }
    }

    #endregion
  }
}
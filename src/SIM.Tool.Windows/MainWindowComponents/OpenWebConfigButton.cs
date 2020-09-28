namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Extensions;

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
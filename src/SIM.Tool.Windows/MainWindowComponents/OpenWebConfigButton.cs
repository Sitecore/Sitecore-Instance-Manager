﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Adapters.WebServer;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Extensions;

  [UsedImplicitly]
  public class OpenWebConfigButton : IMainWindowButton
  {
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
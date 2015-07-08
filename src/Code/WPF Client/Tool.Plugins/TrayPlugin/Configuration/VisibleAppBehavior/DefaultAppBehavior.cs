using System;
using System.Collections.Generic;
using System.Text;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Plugins.TrayPlugin.Common;
using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing;
using SIM.Tool.Plugins.TrayPlugin.WindowZone;

namespace SIM.Tool.Plugins.TrayPlugin.Configuration.VisibleAppBehavior
{
  using SIM.Tool.Windows;

  public class DefaultAppBehavior : AppBehavior
  {
    #region Public Methods and Operators

    public override void Attach()
    {
      AttachToEvents();
    }

    public override void Detach()
    {
      DetachFromEvents();
    }

    #endregion

    #region Methods

    protected virtual void AttachToEvents()
    {
      TrayPluginEvents.ContextMenuExitClick += OnContextMenuExitClick;
      TrayPluginEvents.ContextMenuInstanceEntryClick += OnContextMenuInstanceEntryClick;
      TrayPluginEvents.ContextMenuSIMClick += OnContextMenuSIMClick;
      TrayPluginEvents.TrayIconClick += OnTrayIconClick;
    }


    protected virtual void DetachFromEvents()
    {
      TrayPluginEvents.ContextMenuExitClick -= OnContextMenuExitClick;
      TrayPluginEvents.ContextMenuInstanceEntryClick -= OnContextMenuInstanceEntryClick;
      TrayPluginEvents.ContextMenuSIMClick -= OnContextMenuSIMClick;
      TrayPluginEvents.TrayIconClick -= OnTrayIconClick;
    }

    protected virtual void OnContextMenuExitClick(object sender, ExtendedMouseClickArgs e)
    {
      MouseClickInformation clickInfo = e.ClickInformation;
      if (clickInfo.OnlyLeftMouseButtonPressed)
      {
        WindowWorks.CloseMainWindowToShutdown();
        //LifeManager.ShutdownApplication(KnownShutdownCodes.UserExit);
      }
    }

    protected virtual void OnContextMenuInstanceEntryClick(object sender, InstanceEntryClickArgs e)
    {
      MouseClickInformation clickInfo = e.ClickInformation;
      if (clickInfo.OnlyLeftMouseButtonPressed)
      {
        bool useBackendBrowser = TrayPluginSettingsManager.OpenPagesInBackendBrowser;
        string suffix = TrayPluginSettingsManager.PageToRunOnClick;
        Instance instance = e.Instance;
        InstanceHelperEx.BrowseInstance(instance, MainWindow.Instance, suffix, !useBackendBrowser);
      }
    }

    protected virtual void OnContextMenuSIMClick(object sender, ExtendedMouseClickArgs e)
    {
      MouseClickInformation clickInfo = e.ClickInformation;
      if (clickInfo.OnlyLeftMouseButtonPressed)
      {
        WindowWorks.ShowWindow();
      }
    }

    protected virtual void OnTrayIconClick(object sender, ExtendedMouseClickArgs e)
    {
      MouseClickInformation clickInfo = e.ClickInformation;
      if (clickInfo.OnlyLeftMouseButtonPressed)
      {
        WindowWorks.ShowWindow();
      }
    }

    #endregion
  }
}
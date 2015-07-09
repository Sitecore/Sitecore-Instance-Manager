namespace SIM.Tool.Plugins.TrayPlugin
{
  using System;
  using SIM.Tool.Plugins.TrayPlugin.Common;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing;

  public static class TrayPluginEvents
  {
    #region Public Events

    public static event EventHandler<ConstructedMenuArgs> ContextMenuConstructed
    {
      add
      {
        ContextMenuManager.ActualProvider.ContextMenuConstructed += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.ContextMenuConstructed -= value;
      }
    }

    public static event EventHandler<MenuEntryConstructedArgs> ContextMenuEntryConstructed
    {
      add
      {
        ContextMenuManager.ActualProvider.MenuEntryConstructed += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.MenuEntryConstructed -= value;
      }
    }

    public static event EventHandler<ExtendedMouseClickArgs> ContextMenuExitClick
    {
      add
      {
        ContextMenuManager.ActualProvider.ExitClick += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.ExitClick -= value;
      }
    }

    public static event EventHandler<InstanceEntryClickArgs> ContextMenuInstanceEntryClick
    {
      add
      {
        ContextMenuManager.ActualProvider.InstanceEntryClick += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.InstanceEntryClick -= value;
      }
    }

    public static event EventHandler ContextMenuInvalidated
    {
      add
      {
        ContextMenuManager.ActualProvider.ContextMenuInvalidated += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.ContextMenuInvalidated -= value;
      }
    }

    public static event EventHandler<ExtendedMouseClickArgs> ContextMenuSIMClick
    {
      add
      {
        ContextMenuManager.ActualProvider.SIMClick += value;
      }

      remove
      {
        ContextMenuManager.ActualProvider.SIMClick -= value;
      }
    }

    public static event EventHandler<ExtendedMouseClickArgs> TrayIconClick
    {
      add
      {
        IconManager.ActualProvider.IconClick += value;
      }

      remove
      {
        IconManager.ActualProvider.IconClick -= value;
      }
    }

    public static event EventHandler TrayIconProviderInitialized
    {
      add
      {
        IconManager.ActualProvider.Initialized += value;
      }

      remove
      {
        IconManager.ActualProvider.Initialized -= value;
      }
    }

    #endregion
  }
}
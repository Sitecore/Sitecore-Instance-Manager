namespace SIM.Tool.Plugins.TrayPlugin
{
  using System.Windows;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Plugins.TrayPlugin.Configuration;
  using SIM.Tool.Plugins.TrayPlugin.Configuration.VisibleAppBehavior;
  using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu;
  using SIM.Tool.Plugins.TrayPlugin.WindowZone;

  public class EntryPoint : IInitProcessor, IMainWindowLoadedProcessor
  {
    #region Public methods

    public virtual void Process()
    {
      // Fires lifetime events: Exit, Become visible, config changed.
      LifecycleObserver.Initialize();

      // Register settings in global AdvancedSettings dialog
      TrayPluginSettingsManager.Initialize();

      // Here we define behavior. It subscribes to different events and perform necessary actions (e.g. application exit).
      AppBehaviorManager.Initialize();

      // Subscribes to OnVisible event to patch main window properties.
      WindowWorks.Initialize();

      // Provides context menu, visible thorough the Notify Icon
      ContextMenuManager.Initialize();

      // Adds Notify Icon
      IconManager.Initialize();
    }

    public virtual void Process(Window mainWindow)
    {
      LifecycleObserver.FireVisible(mainWindow);
    }

    #endregion
  }
}
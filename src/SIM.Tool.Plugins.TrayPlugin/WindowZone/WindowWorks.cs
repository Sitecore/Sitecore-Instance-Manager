namespace SIM.Tool.Plugins.TrayPlugin.WindowZone
{
  using System;
  using SIM.Tool.Base.Runtime;
  using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
  using SIM.Tool.Windows;

  public class WindowWorks
  {
    #region Public Properties

    public static WindowServant ActualPrincipal { get; set; }

    #endregion

    #region Public Methods and Operators

    public static void CloseMainWindowToShutdown()
    {
      if (ActualPrincipal == null)
      {
        return;
      }

      ActualPrincipal.CloseWindow();
    }

    public static void DeInitialize()
    {
      if (ActualPrincipal == null)
      {
        return;
      }

      ActualPrincipal.Deinitialize();
    }

    public static void Initialize()
    {
      LifecycleObserver.OnVisible += LifecycleObserver_OnVisible;
    }

    public static void InitializeWithWindow(MainWindow window)
    {
      CreatePrincipalIfNeed();
      ActualPrincipal.InitializeWithWindow(window);
      SubscribeToPopupEvent();
    }

    public static void ShowWindow()
    {
      // Should be performed only after initialization. Ignore attempts before.
      if (ActualPrincipal == null)
      {
        return;
      }

      ActualPrincipal.ShowWindow();
    }

    #endregion

    #region Methods

    #region Protected methods

    protected static void CreatePrincipalIfNeed()
    {
      if (ActualPrincipal != null)
      {
        return;
      }

      ActualPrincipal = new WindowServant();
    }

    protected static void OnWindowPopupRequired(object sender, EventArgs eventArgs)
    {
      LifeManager.UISynchronizationContext.Post(dummy => ShowWindow(), null);
    }

    protected static void SubscribeToPopupEvent()
    {
      LifeManager.OpenAttemptFromAnotherInstance += OnWindowPopupRequired;
    }

    #endregion

    #region Private methods

    private static void LifecycleObserver_OnVisible(object sender, OnVisibleEventArgs e)
    {
      InitializeWithWindow(e.MainWindow);
    }

    #endregion

    #endregion
  }
}
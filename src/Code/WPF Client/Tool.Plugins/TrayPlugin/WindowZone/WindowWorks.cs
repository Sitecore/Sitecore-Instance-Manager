using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
using SIM.Tool.Windows;

namespace SIM.Tool.Plugins.TrayPlugin.WindowZone
{
  public class WindowWorks
  {
    #region Public Properties

    public static WindowServant ActualPrincipal { get; set; }

    #endregion

    #region Public Methods and Operators

    public static void CloseMainWindowToShutdown()
    {
      if (ActualPrincipal == null)
        return;
      ActualPrincipal.CloseWindow();
    }

    public static void DeInitialize()
    {
      if (ActualPrincipal == null)
        return;
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
      //Should be performed only after initialization. Ignore attempts before.
      if (ActualPrincipal == null)
        return;
      ActualPrincipal.ShowWindow();
    }

    #endregion

    #region Methods

    protected static void CreatePrincipalIfNeed()
    {
      if (ActualPrincipal != null)
        return;
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

    private static void LifecycleObserver_OnVisible(object sender, OnVisibleEventArgs e)
    {
      InitializeWithWindow(e.MainWindow);
    }

    #endregion
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Plugins.TrayPlugin.Configuration;
using SIM.Tool.Plugins.TrayPlugin.Lifecycle;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon
{
  public static class IconManager
  {
    #region Static Fields

    private static volatile IconProvider actualProvider;

    #endregion

    #region Public Properties

    public static IconProvider ActualProvider
    {
      //Lazy initialization to allow other principals change the provider. In this case the redundant instance will not be created.
      get
      {
        if (actualProvider != null)
          return actualProvider;
        actualProvider = new IconProvider();
        return actualProvider;
      }
      set
      {
        //Don't see any reason to add the lock here.
        actualProvider = value;
      }
    }

    #endregion

    #region Public Methods and Operators

    public static void Deinitialize()
    {
      if (ActualProvider != null)
      {
        ActualProvider.Deinitialize();
      }
    }

    public static void Initialize()
    {
      if (TrayPluginSettingsManager.DelayedInitialization)
      {
        LifecycleObserver.OnVisible += (sender, args) => InitializeProviderWithoutReservation(LifeManager.UISynchronizationContext, true);
      }
      else
      {
        InitializeProviderWithoutReservation(SynchronizationContext.Current, false);
      }
    }

    #endregion

    #region Methods

    private static void InitializeProviderWithoutReservation(SynchronizationContext contextToInitIn, bool isDelayedInit)
    {
      if (ActualProvider != null)
      {
        ActualProvider.Initialize(contextToInitIn, isDelayedInit);
      }
    }

    #endregion
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Windows;

namespace SIM.Tool.Plugins.TrayPlugin.Lifecycle
{
  public static class LifecycleObserver
  {
    #region Public Events

    public static event EventHandler<ExitEventArgs> OnApplicationExit;
    public static event EventHandler OnConfigChanged;
    public static event EventHandler<OnVisibleEventArgs> OnVisible;

    #endregion

    #region Properties

    private static bool VisibleWasFired { get; set; }

    #endregion

    #region Public Methods and Operators

    public static void FireVisible(Window mainWindow)
    {
      if (!VisibleWasFired)
      {
        if (OnVisible != null)
          OnVisible(null, new OnVisibleEventArgs(mainWindow as MainWindow));
        VisibleWasFired = true;
      }
      else
      {
        if (OnConfigChanged != null)
          OnConfigChanged(null, EventArgs.Empty);
      }
    }

    public static void Initialize()
    {
      Application.Current.Exit += CurrentApplication_Exit;
    }

    #endregion

    #region Methods

    private static void CurrentApplication_Exit(object sender, ExitEventArgs e)
    {
      if (OnApplicationExit != null)
        OnApplicationExit(sender, e);
    }

    #endregion
  }
}
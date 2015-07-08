using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Tool.Plugins.TrayPlugin.Configuration
{
  public static class TrayPluginSettingsManager
  {
    #region Static Fields

    public static readonly string TrayPluginLogStamp = "TrayPlugin";

    #endregion

    #region Constructors and Destructors

    static TrayPluginSettingsManager()
    {
      ActualProvider = new TrayPluginSettingsProvider();
    }

    #endregion

    #region Public Properties

    public static TrayPluginSettingsProvider ActualProvider { get; set; }

    public static bool DelayedInitialization
    {
      get { return ActualProvider.DelayedInitialization; }
      set { ActualProvider.DelayedInitialization = value; }
    }

    public static bool HideWindowFromTaskbar
    {
      get { return ActualProvider.HideWindowFromTaskbar; }
      /*set { ActualProvider.HideWindowFromTaskbar = value; }*/
    }

    public static bool OpenPagesInBackendBrowser
    {
      get { return ActualProvider.OpenPagesInBackendBrowser; }
      /*set { ActualProvider.OpenPagesInBackendBrowser = value; }*/
    }

    public static string PageToRunOnClick
    {
      get { return ActualProvider.PageToRunOnClick; }
      /*set { ActualProvider.PageToRunOnClick = value; }*/
    }

    public static bool PreventApplicationFromBeingClosed
    {
      get { return ActualProvider.PreventApplicationFromBeingClosed; }
      /*set { ActualProvider.PreventApplicationFromBeingClosed = value; }*/
    }

    #endregion

    #region Public Methods and Operators

    public static void Initialize()
    {
      ActualProvider.Initialize();
    }

    #endregion
  }
}
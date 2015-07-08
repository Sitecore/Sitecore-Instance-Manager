using System;
using System.Collections.Generic;
using System.Text;
using SIM.Base;

namespace SIM.Tool.Plugins.TrayPlugin.Configuration
{
  /*
   * The current implementation of this provider returns the constant field. I decided to do such because probably in future there will be a way to customize this.
   */

  public class TrayPluginSettingsProvider
  {
    #region Fields

    protected AdvancedProperty<bool> TrayPluginHideFromTaskbarSetting = AdvancedSettingsManager.CreateAndRegisterBoolSetting("App/Plugins/TrayPlugin/HideFromTaskbar", false);
    protected AdvancedProperty<bool> TrayPluginHideOnClosingSetting = AdvancedSettingsManager.CreateAndRegisterBoolSetting("App/Plugins/TrayPlugin/HideOnClosing", true);
    protected AdvancedProperty<string> TrayPluginUrlSuffixSetting = AdvancedSettingsManager.CreateAndRegisterStringSetting("App/Plugins/TrayPlugin/UrlSuffix", string.Empty);
    protected AdvancedProperty<bool> TrayPluginUseBackendBrowserSetting = AdvancedSettingsManager.CreateAndRegisterBoolSetting("App/Plugins/TrayPlugin/UseBackendBrowser", false);
    protected bool delayedInitialization;

    #endregion

    #region Constructors and Destructors

    public TrayPluginSettingsProvider()
    {
      delayedInitialization = false;
    }

    #endregion

    #region Public Properties

    public bool DelayedInitialization
    {
      get { return delayedInitialization; }
      set { delayedInitialization = value; }
    }

    public bool HideWindowFromTaskbar
    {
      get { return this.TrayPluginHideFromTaskbarSetting.Value; }
    }

    public bool OpenPagesInBackendBrowser
    {
      get { return this.TrayPluginUseBackendBrowserSetting.Value; }
    }

    public string PageToRunOnClick
    {
      get { return this.TrayPluginUrlSuffixSetting.Value; }
    }

    public bool PreventApplicationFromBeingClosed
    {
      get { return this.TrayPluginHideOnClosingSetting.Value; }
    }

    #endregion

    #region Public Methods and Operators

    //It's empty, but we need to call it to create settings field, initialized by constructor.
    public virtual void Initialize()
    {
    }

    #endregion
  }
}
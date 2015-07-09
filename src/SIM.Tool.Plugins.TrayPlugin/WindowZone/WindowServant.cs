using PluginConfig = SIM.Tool.Plugins.TrayPlugin.Configuration.TrayPluginSettingsManager;

namespace SIM.Tool.Plugins.TrayPlugin.WindowZone
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Windows;
  using SIM.Tool.Base.Runtime;
  using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
  using SIM.Tool.Windows;

  public class WindowServant
  {
    #region Properties

    protected bool Initialized { get; set; }
    protected WindowState StateToRestore { get; set; }
    protected MainWindow WindowInstance { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void CloseWindow()
    {
      this.Deinitialize();
      this.WindowInstance.Close();
    }

    public virtual void Deinitialize()
    {
      if (SynchronizationContext.Current != LifeManager.UISynchronizationContext && SynchronizationContext.Current.Equals(LifeManager.UISynchronizationContext))
      {
        this.DeInitializeInContext(null);
      }
      else
      {
        LifeManager.UISynchronizationContext.Send(this.DeInitializeInContext, null);
      }
    }

    public virtual void InitializeWithWindow(MainWindow window)
    {
      this.WindowInstance = window;
      this.TuneWindowProperties();
      LifecycleObserver.OnConfigChanged +=
        (sender, args) => LifeManager.UISynchronizationContext.Send(this.ReInitializeInContext, null);
      this.Initialized = true;
    }

    public virtual void ShowWindow()
    {
      if (!this.Initialized)
      {
        return;
      }

      this.ShowWindowInternal();
    }

    #endregion

    #region Methods

    protected virtual void DeInitializeInContext(object dummy)
    {
      this.RevertTuneWindowToHideFromTaskbar();
      this.RevertTuneWindowToPreventFromBeingClosed();
    }

    protected virtual void HideWindowInternal()
    {
      this.WindowInstance.Hide();
    }

    protected virtual void ReInitializeInContext(object context)
    {
      this.DeInitializeInContext(context);
      this.TuneWindowPropertiesInContext(context);
    }

    protected virtual void RevertTuneWindowToHideFromTaskbar()
    {
      this.WindowInstance.ShowInTaskbar = true;
      this.WindowInstance.StateChanged -= this.WindowInstance_StateChanged;
    }

    protected virtual void RevertTuneWindowToPreventFromBeingClosed()
    {
      ApplicationManager.AttemptToClose -= this.WindowInstance_Closing;
    }

    protected virtual void ShowWindowInternal()
    {
      this.WindowInstance.WindowState = this.StateToRestore;
      this.WindowInstance.Show();
      this.WindowInstance.Activate();
    }

    protected virtual void TuneWindowProperties()
    {
      LifeManager.UISynchronizationContext.Send(this.TuneWindowPropertiesInContext, null);
    }

    protected virtual void TuneWindowPropertiesInContext(object context)
    {
      if (PluginConfig.HideWindowFromTaskbar)
      {
        this.TuneWindowToHideFromTaskbar();
      }

      if (PluginConfig.PreventApplicationFromBeingClosed)
      {
        this.TuneWindowToPreventFromBeingClosed();
      }

      this.StateToRestore = this.WindowInstance.WindowState;
    }

    protected virtual void TuneWindowToHideFromTaskbar()
    {
      this.WindowInstance.ShowInTaskbar = false;
      this.WindowInstance.StateChanged += this.WindowInstance_StateChanged;
    }

    protected virtual void TuneWindowToPreventFromBeingClosed()
    {
      ApplicationManager.AttemptToClose += this.WindowInstance_Closing;
    }

    protected virtual void WindowInstance_Closing(object sender, CancelEventArgs e)
    {
      this.HideWindowInternal();
      e.Cancel = true;
    }

    protected virtual void WindowInstance_StateChanged(object sender, EventArgs e)
    {
      if (this.WindowInstance.WindowState == WindowState.Minimized)
      {
        // Hack to restore the window. Otherwise the window is minimized.
        if (this.StateToRestore == WindowState.Normal)
        {
          this.WindowInstance.WindowState = this.StateToRestore;
        }

        this.WindowInstance.Close();
      }
      else
      {
        this.StateToRestore = this.WindowInstance.WindowState;
      }
    }

    #endregion
  }
}
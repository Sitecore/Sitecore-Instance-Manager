using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SIM.Base;
using SIM.Tool.Plugins.TrayPlugin.Common;
using SIM.Tool.Plugins.TrayPlugin.Helpers;
using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
using SIM.Tool.Plugins.TrayPlugin.Resourcing;
using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon
{
  public class IconProvider
  {
    #region Public Events

    public event EventHandler<ExtendedMouseClickArgs> IconClick;
    public event EventHandler Initialized;

    #endregion

    #region Public Properties

    public NotifyIcon ActualNotifyIcon { get; set; }

    #endregion

    #region Properties

    protected SynchronizationContext ContextToInit { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void Deinitialize()
    {
      if (ActualNotifyIcon != null)
        ActualNotifyIcon.Dispose();
    }

    public virtual void Initialize(SynchronizationContext contextToInitIn, bool isDelayedInit)
    {
      //Check the sychronization context to initialize in WPF UI thread
      ContextToInit = contextToInitIn ?? new SynchronizationContext();
      if (ContextToInit != SynchronizationContext.Current)
      {
        ContextToInit.Send(state => InitializeInContext(isDelayedInit), null);
      }
      else
      {
        InitializeInContext(isDelayedInit);
      }
    }

    #endregion

    #region Methods

    protected virtual void ActualNotifyIconMouseClick(object sender, MouseEventArgs e)
    {
      MouseClickInformation clickInfo = MouseClickInformationHelper.BuildMouseClickInformation(e.Button);
      EventHelper.RaiseEvent(this.IconClick, this, new ExtendedMouseClickArgs(clickInfo));
    }

    protected virtual void BindNotifyIconToMenuProvider()
    {
      TrayPluginEvents.ContextMenuInvalidated += OnContextMenuInvalidated;
      UpdateContextMenuForIcon();
    }

    protected virtual void FinalInitializationSteps()
    {
      BindNotifyIconToMenuProvider();
      EventHelper.RaiseEvent(this.Initialized, this);
    }

    protected virtual Icon GetDefaultTrayIcon()
    {
      var bitmap = new Bitmap(32, 32);
      using (var gc = Graphics.FromImage(bitmap))
      {
        gc.DrawString("S", new Font("Cournier New", 20), Brushes.DarkRed, 2, 2);
      }
      return Icon.FromHandle(bitmap.GetHicon());
    }

    protected virtual NotifyIcon GetInitializedUnvisibleNotifyIcon()
    {
      var notifyIcon = new NotifyIcon() {Visible = false};
      notifyIcon.Text = @"Sitecore Instance Manager";
      notifyIcon.Icon = MultisourceResourcesManager.GetIconResource("trayicon", null) ?? GetDefaultTrayIcon();
      return notifyIcon;
    }

    protected virtual void InitializeInContext(bool isDelayedInit)
    {
      ActualNotifyIcon = GetInitializedUnvisibleNotifyIcon();
      ActualNotifyIcon.Visible = true;
      LifecycleObserver.OnApplicationExit += OnApplicationExit;
      ActualNotifyIcon.MouseClick += ActualNotifyIconMouseClick;
      if (isDelayedInit)
        FinalInitializationSteps();
      else
        LifecycleObserver.OnVisible += OnAppBecomeVisible;
    }

    protected virtual void OnAppBecomeVisible(object sender, EventArgs eventArgs)
    {
      FinalInitializationSteps();
    }

    protected virtual void OnApplicationExit(object sender, ExitEventArgs exitEventArgs)
    {
      Deinitialize();
    }

    protected virtual void OnContextMenuInvalidated(object sender, EventArgs e)
    {
      UpdateContextMenuForIcon();
    }

    protected virtual void UpdateContextMenuForIcon()
    {
      ContextMenuStrip menuStrip = ContextMenuManager.GetContextMenu();
      ActualNotifyIcon.ContextMenuStrip = menuStrip;
    }

    #endregion
  }
}
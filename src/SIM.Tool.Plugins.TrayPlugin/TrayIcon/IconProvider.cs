namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon
{
  using System;
  using System.Drawing;
  using System.Threading;
  using System.Windows;
  using System.Windows.Forms;
  using SIM.Tool.Plugins.TrayPlugin.Common;
  using SIM.Tool.Plugins.TrayPlugin.Helpers;
  using SIM.Tool.Plugins.TrayPlugin.Lifecycle;
  using SIM.Tool.Plugins.TrayPlugin.Resourcing;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu;

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
      if (this.ActualNotifyIcon != null)
      {
        this.ActualNotifyIcon.Dispose();
      }
    }

    public virtual void Initialize(SynchronizationContext contextToInitIn, bool isDelayedInit)
    {
      // Check the sychronization context to initialize in WPF UI thread
      this.ContextToInit = contextToInitIn ?? new SynchronizationContext();
      if (this.ContextToInit != SynchronizationContext.Current)
      {
        this.ContextToInit.Send(state => this.InitializeInContext(isDelayedInit), null);
      }
      else
      {
        this.InitializeInContext(isDelayedInit);
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
      TrayPluginEvents.ContextMenuInvalidated += this.OnContextMenuInvalidated;
      this.UpdateContextMenuForIcon();
    }

    protected virtual void FinalInitializationSteps()
    {
      this.BindNotifyIconToMenuProvider();
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
      var notifyIcon = new NotifyIcon()
      {
        Visible = false
      };
      notifyIcon.Text = @"Sitecore Instance Manager";
      notifyIcon.Icon = MultisourceResourcesManager.GetIconResource("trayicon", null) ?? this.GetDefaultTrayIcon();
      return notifyIcon;
    }

    protected virtual void InitializeInContext(bool isDelayedInit)
    {
      this.ActualNotifyIcon = this.GetInitializedUnvisibleNotifyIcon();
      this.ActualNotifyIcon.Visible = true;
      LifecycleObserver.OnApplicationExit += this.OnApplicationExit;
      this.ActualNotifyIcon.MouseClick += this.ActualNotifyIconMouseClick;
      if (isDelayedInit)
      {
        this.FinalInitializationSteps();
      }
      else
      {
        LifecycleObserver.OnVisible += this.OnAppBecomeVisible;
      }
    }

    protected virtual void OnAppBecomeVisible(object sender, EventArgs eventArgs)
    {
      this.FinalInitializationSteps();
    }

    protected virtual void OnApplicationExit(object sender, ExitEventArgs exitEventArgs)
    {
      this.Deinitialize();
    }

    protected virtual void OnContextMenuInvalidated(object sender, EventArgs e)
    {
      this.UpdateContextMenuForIcon();
    }

    protected virtual void UpdateContextMenuForIcon()
    {
      ContextMenuStrip menuStrip = ContextMenuManager.GetContextMenu();
      this.ActualNotifyIcon.ContextMenuStrip = menuStrip;
    }

    #endregion
  }
}
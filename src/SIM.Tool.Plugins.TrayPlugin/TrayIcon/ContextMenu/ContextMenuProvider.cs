namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Linq;
  using System.Windows.Forms;
  using SIM.Instances;
  using SIM.Tool.Plugins.TrayPlugin.Common;
  using SIM.Tool.Plugins.TrayPlugin.Helpers;
  using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing;
  using Sitecore.Diagnostics;

  public class ContextMenuProvider
  {
    #region Public Events

    public event EventHandler<ConstructedMenuArgs> ContextMenuConstructed;
    public event EventHandler ContextMenuInvalidated;
    public event EventHandler<ExtendedMouseClickArgs> ExitClick;
    public event EventHandler<InstanceEntryClickArgs> InstanceEntryClick;
    public event EventHandler<MenuEntryConstructedArgs> MenuEntryConstructed;
    public event EventHandler<ExtendedMouseClickArgs> SIMClick;

    #endregion

    #region Properties

    protected ContextMenuStrip CachedStrip { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual ContextMenuStrip GetContextMenu()
    {
      if (this.CachedStrip != null)
      {
        return this.CachedStrip;
      }

      this.CachedStrip = this.BuildContextMenu();
      return this.CachedStrip;
    }

    public virtual void Initialize()
    {
      InstanceManager.InstancesListUpdated += this.OnInstancesListUpdated;
    }

    #endregion

    #region Methods

    protected virtual void AddBody(ContextMenuBuilder builder)
    {
      IEnumerable<Instance> instances = this.GetInstancesList();
      foreach (Instance instance in instances)
      {
        this.AddToolStripEntryForInstance(instance, builder);
      }
    }

    protected virtual void AddFooter(ContextMenuBuilder builder)
    {
      builder.AddDemiliter();
      ToolStripItem exitItem = builder.AddItem("Exit", this.ExitClickHandler);
      exitItem.Font = new Font(exitItem.Font, FontStyle.Bold | FontStyle.Italic);
      this.SendItemConstructedEvent(exitItem, MenuEntryPosition.HeaderEntry, null);
    }

    protected virtual void AddHeader(ContextMenuBuilder builder)
    {
      var simItem = builder.AddItem("SIM", this.SIMClickHandler);
      simItem.Font = new Font(simItem.Font, FontStyle.Bold | FontStyle.Italic);
      this.SendItemConstructedEvent(simItem, MenuEntryPosition.HeaderEntry, null);
      builder.AddDemiliter();
    }

    protected virtual void AddToolStripEntryForInstance(Instance instance, ContextMenuBuilder builder)
    {
      var item = builder.AddItem(instance.Name, this.InstanceEntryClickHandler);
      item.Tag = instance;
      this.SendItemConstructedEvent(item, MenuEntryPosition.BodyEntry, instance);
    }

    protected virtual ContextMenuStrip BuildContextMenu()
    {
      var menuBuilder = new ContextMenuBuilder();
      this.AddHeader(menuBuilder);
      this.AddBody(menuBuilder);
      this.AddFooter(menuBuilder);
      ContextMenuStrip resultMenu = menuBuilder.GetResult();
      EventHelper.RaiseEvent(this.ContextMenuConstructed, this, new ConstructedMenuArgs(resultMenu));
      return resultMenu;
    }

    protected virtual void ExitClickHandler(object sender, MouseEventArgs mouseEventArgs)
    {
      MouseClickInformation clickInfo = MouseClickInformationHelper.BuildMouseClickInformation(mouseEventArgs.Button);
      EventHelper.RaiseEvent(this.ExitClick, this, new ExtendedMouseClickArgs(clickInfo));
    }

    protected virtual IEnumerable<Instance> GetInstancesList()
    {
      return InstanceManager.PartiallyCachedInstances.OrderBy(x => x.Name);
    }

    protected virtual void InstanceEntryClickHandler(object sender, MouseEventArgs mouseEventArgs)
    {
      var toolstripItem = (ToolStripItem)sender;
      var instance = toolstripItem.Tag as Instance;
      Assert.IsNotNull(instance, "Instance cannot be null");
      MouseClickInformation clickInfo = MouseClickInformationHelper.BuildMouseClickInformation(mouseEventArgs.Button);
      EventHelper.RaiseEvent(this.InstanceEntryClick, this, new InstanceEntryClickArgs(instance, clickInfo, toolstripItem));
    }

    protected virtual void InvalidateCachedMenu()
    {
      this.CachedStrip = null;
    }

    protected virtual void OnInstancesListUpdated(object sender, EventArgs e)
    {
      this.InvalidateCachedMenu();
      EventHelper.RaiseEvent(this.ContextMenuInvalidated, this);
    }


    protected virtual void SIMClickHandler(object sender, MouseEventArgs mouseEventArgs)
    {
      MouseClickInformation clickInfo = MouseClickInformationHelper.BuildMouseClickInformation(mouseEventArgs.Button);
      EventHelper.RaiseEvent(this.SIMClick, this, new ExtendedMouseClickArgs(clickInfo));
    }

    protected virtual void SendItemConstructedEvent(ToolStripItem item, MenuEntryPosition position, Instance instance)
    {
      var args = new MenuEntryConstructedArgs(item, instance, position);
      EventHelper.RaiseEvent(this.MenuEntryConstructed, this, args);
    }

    #endregion
  }
}
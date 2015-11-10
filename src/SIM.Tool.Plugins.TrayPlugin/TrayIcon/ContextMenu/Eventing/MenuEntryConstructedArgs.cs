namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  using System;
  using System.Windows.Forms;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base.Annotations;

  public class MenuEntryConstructedArgs : EventArgs
  {
    #region Constructors

    public MenuEntryConstructedArgs(ToolStripItem contextMenuItem, Instance instance, MenuEntryPosition position)
    {
      this.ContextMenuItem = contextMenuItem;
      this.Position = position;
      this.Instance = instance;
    }

    #endregion

    #region Public Properties

    public ToolStripItem ContextMenuItem { get; set; }

    [CanBeNull]
    public Instance Instance { get; set; }

    public MenuEntryPosition Position { get; protected set; }

    #endregion
  }
}
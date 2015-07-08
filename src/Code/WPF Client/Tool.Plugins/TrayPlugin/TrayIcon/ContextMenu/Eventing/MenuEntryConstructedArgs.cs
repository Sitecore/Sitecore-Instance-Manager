using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  public class MenuEntryConstructedArgs : EventArgs
  {
    #region Constructors and Destructors

    public MenuEntryConstructedArgs(ToolStripItem contextMenuItem, Instance instance, MenuEntryPosition position)
    {
      ContextMenuItem = contextMenuItem;
      Position = position;
      Instance = instance;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SIM.Instances;
using SIM.Tool.Plugins.TrayPlugin.Common;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  public class InstanceEntryClickArgs : ExtendedMouseClickArgs
  {
    #region Constructors and Destructors

    public InstanceEntryClickArgs(Instance instance, MouseClickInformation clickInformation, ToolStripItem toolStripItem)
      : base(clickInformation)
    {
      this.Instance = instance;
      this.ToolStripItem = toolStripItem;
    }

    #endregion

    #region Public Properties

    public Instance Instance { get; set; }
    public ToolStripItem ToolStripItem { get; set; }

    #endregion
  }
}
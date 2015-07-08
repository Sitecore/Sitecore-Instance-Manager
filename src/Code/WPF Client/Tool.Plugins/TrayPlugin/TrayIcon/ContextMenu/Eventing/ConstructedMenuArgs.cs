using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  public class ConstructedMenuArgs : EventArgs
  {
    #region Constructors and Destructors

    public ConstructedMenuArgs(ContextMenuStrip result)
    {
      Result = result;
    }

    #endregion

    #region Public Properties

    public ContextMenuStrip Result { get; set; }

    #endregion
  }
}
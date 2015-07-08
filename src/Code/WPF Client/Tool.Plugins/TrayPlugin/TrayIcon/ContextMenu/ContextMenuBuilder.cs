using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu
{
  public class ContextMenuBuilder
  {
    #region Constructors and Destructors

    public ContextMenuBuilder()
    {
      ResultContextMenu = new ContextMenuStrip();
    }

    #endregion

    #region Public Properties

    public bool EntryImagesEnabled
    {
      get { return ResultContextMenu.ShowImageMargin; }
      set { ResultContextMenu.ShowImageMargin = value; }
    }

    #endregion

    #region Properties

    protected ContextMenuStrip ResultContextMenu { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void AddDemiliter()
    {
      ResultContextMenu.Items.Add("-");
    }

    public virtual ToolStripItem AddItem(string text, Image image, MouseEventHandler clickHandler)
    {
      var item = ResultContextMenu.Items.Add(text, image);
      item.MouseUp += clickHandler;
      return item;
    }

    public virtual ToolStripItem AddItem(string text, MouseEventHandler clickHandler)
    {
      return AddItem(text, null, clickHandler);
    }

    public virtual ToolStripItem AddItem(string text)
    {
      return AddItem(text, null, null);
    }

    public virtual ContextMenuStrip GetResult()
    {
      return ResultContextMenu;
    }

    #endregion
  }
}
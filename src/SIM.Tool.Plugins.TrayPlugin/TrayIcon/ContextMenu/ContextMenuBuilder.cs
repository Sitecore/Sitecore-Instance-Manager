namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu
{
  using System.Drawing;
  using System.Windows.Forms;

  public class ContextMenuBuilder
  {
    #region Constructors

    public ContextMenuBuilder()
    {
      this.ResultContextMenu = new ContextMenuStrip();
    }

    #endregion

    #region Public Properties

    public bool EntryImagesEnabled
    {
      get
      {
        return this.ResultContextMenu.ShowImageMargin;
      }

      set
      {
        this.ResultContextMenu.ShowImageMargin = value;
      }
    }

    #endregion

    #region Properties

    protected ContextMenuStrip ResultContextMenu { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void AddDemiliter()
    {
      this.ResultContextMenu.Items.Add("-");
    }

    public virtual ToolStripItem AddItem(string text, Image image, MouseEventHandler clickHandler)
    {
      var item = this.ResultContextMenu.Items.Add(text, image);
      item.MouseUp += clickHandler;
      return item;
    }

    public virtual ToolStripItem AddItem(string text, MouseEventHandler clickHandler)
    {
      return this.AddItem(text, null, clickHandler);
    }

    public virtual ToolStripItem AddItem(string text)
    {
      return this.AddItem(text, null, null);
    }

    public virtual ContextMenuStrip GetResult()
    {
      return this.ResultContextMenu;
    }

    #endregion
  }
}
namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  using System;
  using System.Windows.Forms;

  public class ConstructedMenuArgs : EventArgs
  {
    #region Constructors

    public ConstructedMenuArgs(ContextMenuStrip result)
    {
      this.Result = result;
    }

    #endregion

    #region Public Properties

    public ContextMenuStrip Result { get; set; }

    #endregion
  }
}
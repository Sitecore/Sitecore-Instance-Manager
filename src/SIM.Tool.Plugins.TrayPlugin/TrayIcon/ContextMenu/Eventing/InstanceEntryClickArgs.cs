namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu.Eventing
{
  using System.Windows.Forms;
  using SIM.Instances;
  using SIM.Tool.Plugins.TrayPlugin.Common;

  public class InstanceEntryClickArgs : ExtendedMouseClickArgs
  {
    #region Constructors

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
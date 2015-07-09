namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu
{
  using System.Windows.Forms;

  public static class ContextMenuManager
  {
    #region Fields

    private static ContextMenuProvider actualProvider;

    #endregion

    #region Public properties

    public static ContextMenuProvider ActualProvider
    {
      get
      {
        if (actualProvider != null)
        {
          return actualProvider;
        }

        actualProvider = new ContextMenuProvider();
        return actualProvider;
      }

      set
      {
        actualProvider = value;
      }
    }

    #endregion

    #region Public methods

    public static ContextMenuStrip GetContextMenu()
    {
      return ActualProvider.GetContextMenu();
    }

    public static void Initialize()
    {
      ActualProvider.Initialize();
    }

    #endregion
  }
}
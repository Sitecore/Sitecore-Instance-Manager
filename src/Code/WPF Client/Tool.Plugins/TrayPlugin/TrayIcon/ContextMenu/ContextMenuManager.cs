using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu
{
  public static class ContextMenuManager
  {
    private static ContextMenuProvider actualProvider;

    public static ContextMenuProvider ActualProvider
    {
      get
      {
        if (actualProvider != null)
          return actualProvider;
        actualProvider = new ContextMenuProvider();
        return actualProvider;
      }
      set
      {
        actualProvider = value;
      }
    }

    public static ContextMenuStrip GetContextMenu()
    {
      return ActualProvider.GetContextMenu();
    }

    public static void Initialize()
    {
      ActualProvider.Initialize();
    }
  }
}

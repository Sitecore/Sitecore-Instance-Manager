using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SIM.Tool.Windows;

namespace SIM.Tool.Plugins.TrayPlugin.Lifecycle
{
  public class OnVisibleEventArgs : EventArgs
  {
    #region Constructors and Destructors

    public OnVisibleEventArgs(MainWindow mainWindow)
    {
      MainWindow = mainWindow;
    }

    #endregion

    #region Public Properties

    public MainWindow MainWindow { get; set; }

    #endregion
  }
}
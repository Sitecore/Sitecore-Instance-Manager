namespace SIM.Tool.Plugins.TrayPlugin.Lifecycle
{
  using System;
  using SIM.Tool.Windows;

  public class OnVisibleEventArgs : EventArgs
  {
    #region Constructors

    public OnVisibleEventArgs(MainWindow mainWindow)
    {
      this.MainWindow = mainWindow;
    }

    #endregion

    #region Public Properties

    public MainWindow MainWindow { get; set; }

    #endregion
  }
}
namespace SIM.Tool.Plugins.TrayPlugin.Common
{
  using System;

  public class ExtendedMouseClickArgs : EventArgs
  {
    #region Constructors

    public ExtendedMouseClickArgs(MouseClickInformation clickInformation)
    {
      this.ClickInformation = clickInformation;
    }

    #endregion

    #region Public Properties

    public MouseClickInformation ClickInformation { get; set; }

    #endregion
  }
}
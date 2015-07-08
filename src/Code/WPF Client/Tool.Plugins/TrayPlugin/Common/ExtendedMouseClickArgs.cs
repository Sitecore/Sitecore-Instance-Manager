using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Tool.Plugins.TrayPlugin.Helpers;

namespace SIM.Tool.Plugins.TrayPlugin.Common
{
  public class ExtendedMouseClickArgs : EventArgs
  {
    #region Constructors and Destructors

    public ExtendedMouseClickArgs(MouseClickInformation clickInformation)
    {
      ClickInformation = clickInformation;
    }

    #endregion

    #region Public Properties

    public MouseClickInformation ClickInformation { get; set; }

    #endregion
  }
}
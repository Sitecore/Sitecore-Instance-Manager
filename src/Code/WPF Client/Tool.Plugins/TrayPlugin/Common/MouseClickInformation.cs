using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace SIM.Tool.Plugins.TrayPlugin.Common
{
  public class MouseClickInformation
  {
    #region Constructors and Destructors

    public MouseClickInformation(MouseButtons pressedButtons, List<Key> pressedKeyboardKeys)
    {
      MouseButton = pressedButtons;
      PressedKeyboardKeys = pressedKeyboardKeys;
    }

    #endregion

    #region Public Properties

    public MouseButtons MouseButton { get; set; }

    public bool OnlyLeftMouseButtonPressed
    {
      get { return this.MouseButton == MouseButtons.Left && this.PressedKeyboardKeys.Count == 0; }
    }

    public List<Key> PressedKeyboardKeys { get; set; }

    #endregion

    #region Public Methods and Operators

    public bool OnlyMouseButtonPressed(MouseButtons mouseButton)
    {
      return this.MouseButton == mouseButton && this.PressedKeyboardKeys.Count == 0;
    }

    #endregion
  }
}
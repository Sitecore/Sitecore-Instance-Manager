namespace SIM.Tool.Plugins.TrayPlugin.Common
{
  using System.Collections.Generic;
  using System.Windows.Forms;
  using System.Windows.Input;

  public class MouseClickInformation
  {
    #region Constructors

    public MouseClickInformation(MouseButtons pressedButtons, List<Key> pressedKeyboardKeys)
    {
      this.MouseButton = pressedButtons;
      this.PressedKeyboardKeys = pressedKeyboardKeys;
    }

    #endregion

    #region Public Properties

    public MouseButtons MouseButton { get; set; }

    public bool OnlyLeftMouseButtonPressed
    {
      get
      {
        return this.MouseButton == MouseButtons.Left && this.PressedKeyboardKeys.Count == 0;
      }
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
namespace SIM.Tool.Plugins.TrayPlugin.Helpers
{
  using System.Collections.Generic;
  using System.Windows.Forms;
  using System.Windows.Input;
  using SIM.Tool.Plugins.TrayPlugin.Common;

  public static class MouseClickInformationHelper
  {
    #region Public Methods and Operators

    public static MouseClickInformation BuildMouseClickInformation(MouseButtons pressedButton)
    {
      var result = new MouseClickInformation(pressedButton, ExtractPressedKeys());
      return result;
    }

    public static List<Key> ExtractPressedKeys()
    {
      var result = new List<Key>();
      AddIfPressed(Key.LeftCtrl, result);
      AddIfPressed(Key.LeftAlt, result);
      AddIfPressed(Key.LeftShift, result);
      AddIfPressed(Key.RightCtrl, result);
      AddIfPressed(Key.RightAlt, result);
      AddIfPressed(Key.RightShift, result);
      return result;
    }

    #endregion

    #region Methods

    private static void AddIfPressed(Key key, List<Key> output)
    {
      if (Keyboard.IsKeyDown(key))
      {
        output.Add(key);
      }
    }

    #endregion
  }
}
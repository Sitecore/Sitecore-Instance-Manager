namespace SIM.Tool.Base.Windows.Dialogs
{
  using System;
  using System.Windows;
  using System.Windows.Input;
  using JetBrains.Annotations;

  #region

  #endregion

  public partial class InputDialog
  {
    #region Constructors

    public InputDialog()
    {
      InitializeComponent();
    }

    #endregion

    #region Methods

    private void OkClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      DataContext = value.Text;
      DialogResult = true;
      Close();
    }

    #endregion

    #region Private methods

    private void WindowContentRendered(object sender, EventArgs e)
    {
      FocusManager.SetFocusedElement(value, value);
    }

    private void ValueKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        OkClick(null, null);
      }
    }

    #endregion
  }
}
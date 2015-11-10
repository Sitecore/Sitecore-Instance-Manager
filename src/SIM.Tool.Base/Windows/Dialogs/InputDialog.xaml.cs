namespace SIM.Tool.Base.Windows.Dialogs
{
  using System;
  using System.Windows;
  using System.Windows.Input;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public partial class InputDialog
  {
    #region Constructors

    public InputDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    private void OKClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.DataContext = this.value.Text;
      this.DialogResult = true;
      this.Close();
    }

    #endregion

    #region Private methods

    private void WindowContentRendered(object sender, EventArgs e)
    {
      FocusManager.SetFocusedElement(this.value, this.value);
    }

    private void value_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        this.OKClick(null, null);
      }
    }

    #endregion
  }
}
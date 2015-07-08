#region Usings

using System;
using System.Windows;
using System.Windows.Input;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Windows.Dialogs
{
  #region

  

  #endregion

  /// <summary>
  ///   Interaction logic for InputDialog.xaml
  /// </summary>
  public partial class InputDialog
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="InputDialog" /> class.
    /// </summary>
    public InputDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// OKs the click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void OKClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.DataContext = this.value.Text;
      this.DialogResult = true;
      this.Close();
    }

    #endregion

    private void value_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if(e.Key == Key.Enter)
      {
        OKClick(null, null);
      }
    }

    private void WindowContentRendered(object sender, EventArgs e)
    {
      FocusManager.SetFocusedElement(value, value);
    }
  }
}
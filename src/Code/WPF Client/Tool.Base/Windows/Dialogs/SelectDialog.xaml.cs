#region Usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Windows.Dialogs
{
  #region

  

  #endregion

  /// <summary>
  ///   The select dialog.
  /// </summary>
  public partial class SelectDialog
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SelectDialog" /> class.
    /// </summary>
    public SelectDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Cancels the click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void CancelClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.DataContext = null;
      this.DialogResult = false;
    }

    /// <summary>
    /// Lists the mouse double click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data. 
    /// </param>
    private void ListMouseDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      this.OKClick(sender, e);
    }

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
      string s = AllowMultiSelect ? this.ListBox1.SelectedItems.Cast<string>().Aggregate(string.Empty, (current, selectedItem) => current + (selectedItem + ',')) : (string)this.ListBox1.SelectedItem;
      this.DataContext = s.Trim(',');
      this.DialogResult = true;
    }

    /// <summary>
    /// Windows the key up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key == Key.Escape)
      {
        if (e.Handled) return;
        e.Handled = true;
        this.CancelClick(sender, e);
      }
      else if (e.Key == Key.Enter)
      {
        OKClick(null, null);
      }
    }

    #endregion

    public bool AllowMultiSelect
    {
      get { return this.ListBox1.SelectionMode == SelectionMode.Multiple; }
      set { this.ListBox1.SelectionMode = value ? SelectionMode.Multiple : SelectionMode.Single; }
    }

    public string DefaultValue { get; set; }

    private void WindowContentRendered(object sender, EventArgs e)
    {
      FocusManager.SetFocusedElement(ListBox1, ListBox1);
      if (AllowMultiSelect)
      {
        ListBox1.SelectedItems.Add(DefaultValue);
      }
      else
      {
        ListBox1.SelectedItem = DefaultValue;
      }
      var t = this.Title;
      this.Title = "Select an option";
      this.Label.Text = t;
    }
  }
}
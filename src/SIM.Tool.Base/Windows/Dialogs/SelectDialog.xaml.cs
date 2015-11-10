namespace SIM.Tool.Base.Windows.Dialogs
{
  using System;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public partial class SelectDialog
  {
    #region Constructors

    public SelectDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.DataContext = null;
      this.DialogResult = false;
    }

    private void ListMouseDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      this.OKClick(sender, e);
    }

    private void OKClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      string s = this.AllowMultiSelect ? this.ListBox1.SelectedItems.Cast<string>().Aggregate(string.Empty, (current, selectedItem) => current + (selectedItem + ',')) : (string)this.ListBox1.SelectedItem;
      this.DataContext = s.Trim(',');
      this.DialogResult = true;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        this.CancelClick(sender, e);
      }
      else if (e.Key == Key.Enter)
      {
        this.OKClick(null, null);
      }
    }

    #endregion

    #region Public properties

    public bool AllowMultiSelect
    {
      get
      {
        return this.ListBox1.SelectionMode == SelectionMode.Multiple;
      }

      set
      {
        this.ListBox1.SelectionMode = value ? SelectionMode.Multiple : SelectionMode.Single;
      }
    }

    public string DefaultValue { get; set; }

    #endregion

    #region Private methods

    private void WindowContentRendered(object sender, EventArgs e)
    {
      FocusManager.SetFocusedElement(this.ListBox1, this.ListBox1);
      if (this.AllowMultiSelect)
      {
        this.ListBox1.SelectedItems.Add(this.DefaultValue);
      }
      else
      {
        this.ListBox1.SelectedItem = this.DefaultValue;
      }

      var t = this.Title;
      this.Title = "Select an option";
      this.Label.Text = t;
    }

    #endregion
  }
}
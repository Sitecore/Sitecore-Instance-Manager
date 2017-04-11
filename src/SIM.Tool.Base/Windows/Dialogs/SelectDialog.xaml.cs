namespace SIM.Tool.Base.Windows.Dialogs
{
  using System;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public partial class SelectDialog
  {
    #region Constructors

    public SelectDialog()
    {
      InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      DataContext = null;
      DialogResult = false;
    }

    private void ListMouseDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      OkClick(sender, e);
    }

    private void OkClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      var s = AllowMultiSelect ? ListBox1.SelectedItems.Cast<string>().Aggregate(string.Empty, (current, selectedItem) => current + (selectedItem + ',')) : (string)ListBox1.SelectedItem;
      DataContext = s.Trim(',');
      DialogResult = true;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        CancelClick(sender, e);
      }
      else if (e.Key == Key.Enter)
      {
        OkClick(null, null);
      }
    }

    #endregion

    #region Public properties

    public bool AllowMultiSelect
    {
      get
      {
        return ListBox1.SelectionMode == SelectionMode.Multiple;
      }

      set
      {
        ListBox1.SelectionMode = value ? SelectionMode.Multiple : SelectionMode.Single;
      }
    }

    public string DefaultValue { get; set; }

    #endregion

    #region Private methods

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

      var t = Title;
      Title = "Select an option";
      Label.Text = t;
    }

    #endregion
  }
}
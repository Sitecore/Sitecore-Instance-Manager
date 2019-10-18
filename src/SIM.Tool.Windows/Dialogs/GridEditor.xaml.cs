using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SIM.Tool.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for GridEditor.xaml
  /// </summary>
  public partial class GridEditor : Window
  {
    public GridEditor()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      Assert.ArgumentNotNull(editContext, nameof(editContext));
      this.DescriptionText.Text = editContext.Description;
      
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      var errors = (from c in
            (from object i in DataGrid.ItemsSource
              select DataGrid.ItemContainerGenerator.ContainerFromItem(i))
          where c != null
          select Validation.GetHasError(c))
        .FirstOrDefault(x => x);
      this.DialogResult = !errors;
      this.Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Button b = sender as Button;
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      editContext.GridItems.Remove(b.DataContext);
      this.DataGrid.DataContext = null;
      this.DataGrid.DataContext = editContext;
    }
  }

  internal class GridObjectValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      IValidateable entry = (value as BindingGroup).Items[0] as IValidateable;
      if (entry == null)
      {
        return ValidationResult.ValidResult;
      }

      string error = entry.ValidateAndGetError();
      if (string.IsNullOrEmpty(error))
      {
        return ValidationResult.ValidResult;
      }

      return new ValidationResult(false, error);
    }
  }
}

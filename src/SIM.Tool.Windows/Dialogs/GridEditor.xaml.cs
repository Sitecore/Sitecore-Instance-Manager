using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
      
      //Bind properties
      IEnumerable<PropertyInfo> propertiesToRender = editContext.ElementType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RenderInDataGreedAttribute)));
      foreach (var propertyToRender in propertiesToRender)
      {
        this.DataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding(propertyToRender.Name), Header = propertyToRender.Name});
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      var errors = (from c in
            (from object i in DataGrid.ItemsSource
              select DataGrid.ItemContainerGenerator.ContainerFromItem(i))
          where c != null
          select Validation.GetHasError(c))
        .FirstOrDefault(x => x);

      //In case of empty row was added but wasn't changed. Just remove it.
      if (!errors)
      {
        GridEditorContext editContext = this.DataContext as GridEditorContext;
        for (int i = 0; i < editContext.GridItems.Count; i++)
        {
          if ((editContext.GridItems[i] as IValidateable).HasAnyValuesInTheFields())
          { 
            continue;
          }
          editContext.GridItems.RemoveAt(i);
          i--;
        }
      }

      if (errors)
      {
        if (MessageBox.Show("There are validation errors. Data will not be saved.\nProceed anyway?", "Invalid data",
              MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
          return;
        }
      }
      this.DialogResult = !errors;
      this.Close();
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
      Button b = sender as Button;
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      editContext.GridItems.Remove(b.DataContext);
      this.DataGrid.DataContext = null;
      this.DataGrid.DataContext = editContext;
    }

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
      Button b = sender as Button;
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      editContext.GridItems.Add(Activator.CreateInstance(editContext.ElementType));
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

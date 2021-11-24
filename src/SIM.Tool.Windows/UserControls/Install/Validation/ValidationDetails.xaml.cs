using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SIM.Tool.Windows.Dialogs;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.UserControls.Install.Validation
{
  /// <summary>
  /// Interaction logic for Install9ParametersEditor.xaml
  /// </summary>
  public partial class ValidationDetails : Window
  {
    public ValidationDetails()
    {
      InitializeComponent();
    }

    private void Btn_Close_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void ValidationDetails_OnLoaded(object sender, RoutedEventArgs e)
    {
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      Assert.ArgumentNotNull(editContext, nameof(editContext));

      // Bind properties to be able to copy and paste data from DataGrid
      IEnumerable<PropertyInfo> propertiesToRender = editContext.ElementType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RenderInDataGreedAttribute)));
      foreach (var propertyToRender in propertiesToRender)
      {
        if (propertyToRender.Name == "State")
        {
          this.MessagesList.Columns.Add(new DataGridTextColumn() { Binding = new Binding(propertyToRender.Name), Header = propertyToRender.Name, IsReadOnly = true, MinWidth = 58, MaxWidth = 58 });
        }
        else
        {
          this.MessagesList.Columns.Add(new DataGridTextColumn() { Binding = new Binding(propertyToRender.Name) { Mode = BindingMode.OneTime }, Header = propertyToRender.Name, 
            ElementStyle = this.GetElementStyle(), EditingElementStyle = this.GetEditingElementStyle() });
        }
      }

      this.UpdateDataGridRowColor(); 
    }

    private Style GetElementStyle()
    {
      Style elementStyle = new Style(typeof(TextBlock));
      Setter elementStyleSetter = new Setter()
      {
        Property = TextBlock.TextWrappingProperty,
        Value = TextWrapping.Wrap
      };
      elementStyle.Setters.Add(elementStyleSetter);
      return elementStyle;
    }

    private Style GetEditingElementStyle()
    {
      Style editingElementStyle = new Style(typeof(TextBox));
      Setter editingElementStyleSetter = new Setter()
      {
        Property = TextBox.TextWrappingProperty,
        Value = TextWrapping.Wrap
      };
      editingElementStyle.Setters.Add(editingElementStyleSetter);
      return editingElementStyle;
    }

    private void UpdateDataGridRowColor()
    {
      this.MessagesList.UpdateLayout();
      for (int i = 0; i < this.MessagesList.Items.Count; i++)
      {
        DataGridRow row = (DataGridRow)this.MessagesList.ItemContainerGenerator.ContainerFromIndex(i);
        if (row != null)
        {
          Sitecore9Installer.Validation.ValidatorState validatorState = (this.MessagesList.Items[i] as Sitecore9Installer.Validation.ValidationResult).State;
          switch (validatorState)
          {
            case Sitecore9Installer.Validation.ValidatorState.Success:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ccffcc");
              break;
            }
            case Sitecore9Installer.Validation.ValidatorState.Error:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#e86f6d");
              break;
            }
            case Sitecore9Installer.Validation.ValidatorState.Warning:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#e8e39a");
              break;
            }
            case Sitecore9Installer.Validation.ValidatorState.Pending:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#e8e39a");
              break;
            }
            default: break;
          }
        }
      }
    }
  }
}

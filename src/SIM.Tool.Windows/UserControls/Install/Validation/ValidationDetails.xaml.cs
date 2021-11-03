using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

      //Bind properties to be able to copy and paste
      IEnumerable<PropertyInfo> propertiesToRender = editContext.ElementType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RenderInDataGreedAttribute)));
      foreach (var propertyToRender in propertiesToRender)
      {
        this.MessagesList.Columns.Add(new DataGridTextColumn() { Binding = new Binding(propertyToRender.Name), Header = propertyToRender.Name });
      }
    }
  }
}

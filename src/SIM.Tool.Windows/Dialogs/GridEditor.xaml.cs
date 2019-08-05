using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
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
      this.DataGrid.ItemsSource = editContext.GridItems;
      this.DescriptionText.Text = editContext.Description;
    }   

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}

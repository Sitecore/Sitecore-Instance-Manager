using SIM.ContainerInstaller;
using Sitecore.Diagnostics.Base;
using System.Collections.Generic;
using System.Windows;

namespace SIM.Tool.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for GridEditor.xaml
  /// </summary>
  public partial class ContainerVariablesEditor : Window
  {
    public ContainerVariablesEditor()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      List<NameValuePair> editContext = this.DataContext as List<NameValuePair>;
      Assert.ArgumentNotNull(editContext, nameof(editContext));
      
      //Bind properties
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {     
      this.Close();
    }

    private void ReinitializeDataContext(GridEditorContext editContext)
    {
      this.DataGrid.DataContext = null;
      this.DataGrid.DataContext = editContext;
    }
  }
}

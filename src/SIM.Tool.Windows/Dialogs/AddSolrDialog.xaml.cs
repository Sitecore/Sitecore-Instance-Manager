using SIM.Tool.Base;
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
  /// Interaction logic for AddSolrDialog.xaml
  /// </summary>
  public partial class AddSolrDialog : Window
  {
    public AddSolrDialog()
    {
      InitializeComponent();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      SolrDefinition solr = new SolrDefinition();
      solr.Name = this.NameText.Text;
      solr.Root = this.RootText.Text;
      solr.Url = this.UrlText.Text;
      if(string.IsNullOrWhiteSpace(solr.Name)|| string.IsNullOrWhiteSpace(solr.Url)|| string.IsNullOrWhiteSpace(solr.Root))
      {
        MessageBox.Show("Name, Root and Url must not be empty.");
        return;
      }
      solr.Service = this.ServiceText.Text;
      this.DataContext = solr;
      this.DialogResult = true;
      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }   

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Select solr root folder", this.RootText, null, null);
    }
  }
}

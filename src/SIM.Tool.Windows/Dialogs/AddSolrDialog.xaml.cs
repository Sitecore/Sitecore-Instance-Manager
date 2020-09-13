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
using SIM.Tool.Base.Profiles;
using SIM.Tool.Windows.MainWindowComponents;

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
      solr.Service = this.ServiceText.Text;
      string error = solr.ValidateAndGetError();
      if (!string.IsNullOrEmpty(error))
      {
        MessageBox.Show(error);
        return;
      }

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

    private void InstallSolr_OnClick(object sender, RoutedEventArgs e)
    {
      int solrsCount = ProfileManager.Profile.Solrs.Count;
      InstallSolrButton installSolrButton = new InstallSolrButton();
      // Reinitialize data context and close the Solr dialog box after installing the new Solr server
      installSolrButton.InstallationCompleted += (o, args) =>
      {
        if (solrsCount < ProfileManager.Profile.Solrs.Count)
        {
          this.DataContext = ProfileManager.Profile.Solrs.Last();
          this.DialogResult = true;
          this.Close();
        }
        else
        {
          this.Cancel_Click(null, null);
        }
      };
      installSolrButton.InstallSolr(this, false);
    }
  }
}

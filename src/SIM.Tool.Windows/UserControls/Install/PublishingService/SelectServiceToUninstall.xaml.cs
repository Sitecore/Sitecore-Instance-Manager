using Microsoft.Web.Administration;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.IO;
using System.Linq;

namespace SIM.Tool.Windows.UserControls.Install.PublishingService
{
  /// <summary>
  /// Interaction logic for SelectServiceToUninstall.xaml
  /// </summary>
  public partial class SelectServiceToUninstall : IWizardStep, IFlowControl
  {
    private string InstanceFolder { get; set; }
    public SelectServiceToUninstall()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      UninstallSPSWizardArgs args = (UninstallSPSWizardArgs)wizardArgs;
      this.InstanceFolder = args.SPSInstanceFolder;

      

      using (ServerManager sm = new ServerManager())
      {
        Site spsSite = sm.Sites.FirstOrDefault(s => s.Name.Equals(args.InstanceName));
        SiteNameTextBox.Text = spsSite?.Name ?? args.InstanceName;
        AppPoolTextBox.Text = spsSite?.Applications.FirstOrDefault()?.ApplicationPoolName  ?? args.InstanceName;
        WebrootTextBox.Text = args.Instance.WebRootPath;
      }

      return;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      UninstallSPSWizardArgs args = (UninstallSPSWizardArgs)wizardArgs;
      args.SkipSPSSite = SkipSiteName.IsChecked ?? false;
      args.SkipSPSAppPool = SkipAppPool.IsChecked ?? false;
      args.SkipSPSWebroot = SkipWebrootFolder.IsChecked ?? false;

      Site site = null;
      ApplicationPool appPool = null;
      string webroot = "";

      using (ServerManager sm = new ServerManager())
      {
        site = sm.Sites.FirstOrDefault(s => s.Name.Equals(SiteNameTextBox.Text));
        appPool = sm.ApplicationPools.FirstOrDefault(a => a.Name.Equals(AppPoolTextBox.Text));
      }
      webroot = WebrootTextBox.Text;

      if (site == null && !args.SkipSPSSite)
      {
          WindowHelper.ShowMessage($"The site {SiteNameTextBox.Text} was not found in IIS");
          return false;
      }

      if (appPool == null && !args.SkipSPSAppPool)
      {
        WindowHelper.ShowMessage($"The application pool {AppPoolTextBox.Text} was not found in IIS");
        return false;
      }

      if (!Directory.Exists(webroot) && !args.SkipSPSWebroot)
      {
        WindowHelper.ShowMessage($"The {webroot} directory does not exist");
        return false;
      }

      args.SPSSiteName = site?.Name ?? "";
      args.SPSAppPoolName = appPool?.Name ?? "";
      args.SPSWebroot = webroot;
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    private void SiteNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      AppPoolTextBox.Text = SiteNameTextBox.Text;
      WebrootTextBox.Text = Path.Combine(InstanceFolder, SiteNameTextBox.Text);
    }

    private void SkipSiteName_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
      SiteNameTextBox.IsEnabled = !(SkipSiteName.IsChecked ?? false);
    }

    private void SkipAppPool_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
      AppPoolTextBox.IsEnabled = !(SkipAppPool.IsChecked ?? false);
    }

    private void SkipWebrootFolder_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
      WebrootTextBox.IsEnabled = !(SkipWebrootFolder.IsChecked ?? false);
    }
  }
}

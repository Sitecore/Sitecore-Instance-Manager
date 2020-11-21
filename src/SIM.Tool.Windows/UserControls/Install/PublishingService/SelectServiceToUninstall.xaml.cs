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
    public SelectServiceToUninstall()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      UninstallSPSWizardArgs args = (UninstallSPSWizardArgs)wizardArgs;
      if (string.IsNullOrEmpty(SiteNameTextBox.Text))
      {
        SiteNameTextBox.Text = "sitecore.publishing";
      }
      if (string.IsNullOrEmpty(AppPoolTextBox.Text))
      {
        AppPoolTextBox.Text = "sitecore.publishing";
      }
      if (string.IsNullOrEmpty(WebrootTextBox.Text))
      {
        WebrootTextBox.Text = Path.Combine(args.SPSInstanceFolder, "sitecore.publishing");
      }
      return;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Site site;
      ApplicationPool appPool;
      string webroot;

      using (ServerManager sm = new ServerManager())
      {
        site = sm.Sites.FirstOrDefault(s => s.Name.Equals(SiteNameTextBox.Text));
        appPool = sm.ApplicationPools.FirstOrDefault(a => a.Name.Equals(AppPoolTextBox.Text));
      }
      webroot = WebrootTextBox.Text;

      if (site == null)
      {
        WindowHelper.ShowMessage($"The site {site.Name} was not found in IIS");
        return false;
      }

      if (appPool == null)
      {
        WindowHelper.ShowMessage($"The site {appPool.Name} was not found in IIS");
        return false;
      }

      if (!Directory.Exists(webroot))
      {
        WindowHelper.ShowMessage($"The {webroot} directory does not exist");
      }
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }
  }
}

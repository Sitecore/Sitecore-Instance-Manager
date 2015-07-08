using System.IO;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System.Windows;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.UserControls.Import
{
  public partial class ImportWebsite : IWizardStep
  {

    string rootFolderToInstall = "";

    public ImportWebsite()
    {
      InitializeComponent();
    }

    
    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
      websiteName.Text = args.siteName;
      rootFolderPath.Text = args.rootPath;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (ImportWizardArgs)wizardArgs;
      if (WebServerManager.WebsiteExists(websiteName.Text))
      {
        websiteNameWarn.Visibility = System.Windows.Visibility.Visible;
        return false;
      }

      websiteNameWarn.Visibility = System.Windows.Visibility.Hidden;

      if (FileSystem.Local.Directory.Exists(rootFolderPath.Text) && FileSystem.Local.Directory.GetFiles(rootFolderPath.Text).Length > 0)
      {
        rootPathWarn.Visibility = System.Windows.Visibility.Visible;
        return false;
      }

        
        args.siteName = websiteName.Text;
        args.rootPath = rootFolderPath.Text;
        args.updateLicense = updateLicense.IsChecked==true ? true : false ;
        args.pathToLicenseFile = ProfileManager.Profile.License;
        return true;    
    }

    private void PickRootFolderClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {

      WindowHelper.PickFolder("Select root folder to install:", rootFolderPath, null);
        /*if (result == true)
        {
          rootFolderToInstall = Path.GetDirectoryName(fileDialog.FileName);
        }*/
    }
    

  }
}

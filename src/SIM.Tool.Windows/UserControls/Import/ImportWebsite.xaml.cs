namespace SIM.Tool.Windows.UserControls.Import
{
  using System.Windows;
  using SIM.Adapters.WebServer;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base.Annotations;

  public partial class ImportWebsite : IWizardStep
  {
    #region Fields

    private string rootFolderToInstall = string.Empty;

    #endregion

    #region Constructors

    public ImportWebsite()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
      this.websiteName.Text = args.siteName;
      this.rootFolderPath.Text = args.rootPath;
    }

    private void PickRootFolderClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Select root folder to install:", this.rootFolderPath, null);

      /*if (result == true)
        {
          rootFolderToInstall = Path.GetDirectoryName(fileDialog.FileName);
        }*/
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (ImportWizardArgs)wizardArgs;
      if (WebServerManager.WebsiteExists(this.websiteName.Text))
      {
        this.websiteNameWarn.Visibility = System.Windows.Visibility.Visible;
        return false;
      }

      this.websiteNameWarn.Visibility = System.Windows.Visibility.Hidden;

      if (FileSystem.FileSystem.Local.Directory.Exists(this.rootFolderPath.Text) && FileSystem.FileSystem.Local.Directory.GetFiles(this.rootFolderPath.Text).Length > 0)
      {
        this.rootPathWarn.Visibility = System.Windows.Visibility.Visible;
        return false;
      }


      args.siteName = this.websiteName.Text;
      args.rootPath = this.rootFolderPath.Text;
      args.updateLicense = this.updateLicense.IsChecked == true ? true : false;
      args.pathToLicenseFile = ProfileManager.Profile.License;
      return true;
    }

    #endregion
  }
}
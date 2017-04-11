namespace SIM.Tool.Windows.UserControls.Import
{
  using System.Windows;
  using SIM.Adapters.WebServer;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using JetBrains.Annotations;

  public partial class ImportWebsite : IWizardStep
  {
    #region Fields

    private string _RootFolderToInstall = string.Empty;

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
      this.websiteName.Text = args._SiteName;
      this.rootFolderPath.Text = args._RootPath;
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


      args._SiteName = this.websiteName.Text;
      args._RootPath = this.rootFolderPath.Text;
      args._UpdateLicense = this.updateLicense.IsChecked == true ? true : false;
      args._PathToLicenseFile = ProfileManager.Profile.License;
      return true;
    }

    #endregion
  }
}
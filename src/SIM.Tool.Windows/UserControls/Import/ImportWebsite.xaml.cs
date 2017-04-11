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
      InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
      websiteName.Text = args._SiteName;
      rootFolderPath.Text = args._RootPath;
    }

    private void PickRootFolderClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Select root folder to install:", rootFolderPath, null);

      /*if (result == true)
        {
          rootFolderToInstall = Path.GetDirectoryName(fileDialog.FileName);
        }*/
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

      if (FileSystem.FileSystem.Local.Directory.Exists(rootFolderPath.Text) && FileSystem.FileSystem.Local.Directory.GetFiles(rootFolderPath.Text).Length > 0)
      {
        rootPathWarn.Visibility = System.Windows.Visibility.Visible;
        return false;
      }


      args._SiteName = websiteName.Text;
      args._RootPath = rootFolderPath.Text;
      args._UpdateLicense = updateLicense.IsChecked == true ? true : false;
      args._PathToLicenseFile = ProfileManager.Profile.License;
      return true;
    }

    #endregion
  }
}
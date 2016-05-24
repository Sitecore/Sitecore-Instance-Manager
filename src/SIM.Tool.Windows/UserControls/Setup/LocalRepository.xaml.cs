namespace SIM.Tool.Windows.UserControls.Setup
{
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Setup;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  public partial class LocalRepository : IWizardStep, IFlowControl
  {
    #region Constructors

    public LocalRepository()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Private methods

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      var repository = args.LocalRepositoryFolderPath;
      var one = !string.IsNullOrEmpty(repository) && FileSystem.FileSystem.Local.Directory.Exists(repository);
      if (!one)
      {
        WindowHelper.ShowMessage("The Local Repository folder does not exist, please choose existing folder", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
        return false;
      }

      var license = args.LicenseFilePath;
      var two = !string.IsNullOrEmpty(license) && FileSystem.FileSystem.Local.File.Exists(license);
      if (!two)
      {
        WindowHelper.ShowMessage("The Sitecore License file does not exist, please choose existing file", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
        return false;
      }

      return true;
    }

    private void PickLicenseFileClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFile("Choose the Sitecore license file", this.LicenseFile, this.PickLicenseFile, "*.xml|*.xml");
    }

    private void PickLocalRepositoryFolderClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose the Local Repository folder", this.LocalRepositoryFolder, this.PickLocalRepositoryFolder);
    }

    private void ShowMoreInfo([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      CoreApp.OpenInBrowser("https://bitbucket.org/sitecore/sitecore-instance-manager/wiki/Manual-Download", true);
    }

    private void ShowSupportedModules([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      CoreApp.OpenInBrowser("https://bitbucket.org/sitecore/sitecore-instance-manager/wiki/Home", true);
    }

    #endregion

    #region IWizardStep Members

    private static string GetRepositoryPath()
    {
      string path = Path.Combine(ApplicationManager.DataFolder, "Repository");
      FileSystem.FileSystem.Local.Directory.Ensure(path);
      return path;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.LicenseFile.Text = args.LicenseFilePath;
      this.LocalRepositoryFolder.Text = args.LocalRepositoryFolderPath.EmptyToNull() ?? GetRepositoryPath();
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      args.LicenseFilePath = this.LicenseFile.Text;
      args.LocalRepositoryFolderPath = this.LocalRepositoryFolder.Text;
      return true;
    }

    #endregion
  }
}
using System.IO;
using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Base.Wizards;
using System.Windows;
using SIM.Tool.Windows.Pipelines.Setup;

namespace SIM.Tool.Windows.UserControls.Setup
{
  /// <summary>
  /// Interaction logic for LocalRepository.xaml
  /// </summary>
  public partial class LocalRepository : IWizardStep, IFlowControl
  {
    public LocalRepository()
    {
      InitializeComponent();
    }

    private void PickLocalRepositoryFolderClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose the Local Repository folder", this.LocalRepositoryFolder, this.PickLocalRepositoryFolder);
    }

    private void PickLicenseFileClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFile("Choose the Sitecore license file", this.LicenseFile, this.PickLicenseFile, "*.xml|*.xml");
    }

    private void ShowSupportedModules([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.OpenInBrowser("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Home", true);
    }

    private void ShowMoreInfo([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.OpenInBrowser("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Manual-Download", true);
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      var repository = args.LocalRepositoryFolderPath;
      var one = !string.IsNullOrEmpty(repository) && FileSystem.Local.Directory.Exists(repository);
      if (!one)
      {
          WindowHelper.ShowMessage("The Local Repository folder does not exist, please choose existing folder", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
        return false;
      }

      var license = args.LicenseFilePath;
      var two = !string.IsNullOrEmpty(license) && FileSystem.Local.File.Exists(license);
      if (!two)
      {
          WindowHelper.ShowMessage("The Sitecore License file does not exist, please choose existing file", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
        return false;
      }

      return true;
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }          

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.LicenseFile.Text = args.LicenseFilePath;
      this.LocalRepositoryFolder.Text = args.LocalRepositoryFolderPath.EmptyToNull() ?? GetRepositoryPath(); 
    }

    private static string GetRepositoryPath()
    {
      string path = Path.Combine(ApplicationManager.DataFolder, "Repository");
      FileSystem.Local.Directory.Ensure(path);
      return path;
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

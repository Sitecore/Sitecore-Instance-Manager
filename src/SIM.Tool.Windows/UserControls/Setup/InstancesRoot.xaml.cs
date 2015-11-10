namespace SIM.Tool.Windows.UserControls.Setup
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Setup;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public partial class InstancesRoot : IWizardStep, IFlowControl
  {
    #region Constructors

    public InstancesRoot()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public methods

    public bool OnMovingNext(SetupWizardArgs args)
    {
      var folderPath = args.InstancesRootFolderPath;

      if (string.IsNullOrEmpty(folderPath))
      {
        return false;
      }

      if (folderPath.Length < 3 || !folderPath[1].Equals(':') || !folderPath[2].Equals('\\'))
      {
        WindowHelper.ShowMessage("The path must be rooted e.g. C:\\inetpub\\wwwroot or or D:\\", MessageBoxButton.OK, 
          MessageBoxImage.Hand, MessageBoxResult.OK);
        return false;
      }

      string fullPath = null;
      try
      {
        fullPath = Path.GetFullPath(folderPath);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during moving next in InstancesRoot.xaml.cs");
        WindowHelper.ShowMessage("The specified path is not valid", MessageBoxButton.OK, MessageBoxImage.Hand, 
          MessageBoxResult.OK);
        return false;
      }

      FileSystem.FileSystem.Local.Directory.Ensure(fullPath);
      args.InstancesRootFolderPath = fullPath;

      return true;
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.MainRootFolder.Text = args.InstancesRootFolderPath.EmptyToNull() ?? "C:\\inetpub\\wwwroot";
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      return this.OnMovingNext(args);
    }

    private void PickInstancesFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose the Instances Root folder", this.MainRootFolder, this.PickInstancesFolderButton);
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      args.InstancesRootFolderPath = this.MainRootFolder.Text.Trim();
      return true;
    }

    #endregion
  }
}
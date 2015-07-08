using System;
using System.IO;
using System.Windows;
using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Pipelines.Setup;

namespace SIM.Tool.Windows.UserControls.Setup
{
  /// <summary>
  /// Interaction logic for InstancesRoot.xaml
  /// </summary>
  public partial class InstancesRoot : IWizardStep, IFlowControl
  {
    public InstancesRoot()
    {
      InitializeComponent();
    }

    private void PickInstancesFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose the Instances Root folder", this.MainRootFolder, this.PickInstancesFolderButton);
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      return OnMovingNext(args);
    }

    public bool OnMovingNext(SetupWizardArgs args)
    {
      var folderPath = args.InstancesRootFolderPath;

      if (string.IsNullOrEmpty(folderPath)) return false;

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
      catch(Exception ex)
      {
        Log.Warn("An error occurred during moving next in InstancesRoot.xaml.cs", this, ex);
        WindowHelper.ShowMessage("The specified path is not valid", MessageBoxButton.OK, MessageBoxImage.Hand,
                                 MessageBoxResult.OK);
        return false;
      }

      SIM.Base.FileSystem.Local.Directory.Ensure(fullPath);
      args.InstancesRootFolderPath = fullPath;

      return true;
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.MainRootFolder.Text = args.InstancesRootFolderPath.EmptyToNull() ?? "C:\\inetpub\\wwwroot";
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      args.InstancesRootFolderPath = this.MainRootFolder.Text.Trim();
      return true;
    }
  }
}

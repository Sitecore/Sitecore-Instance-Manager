using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class Install9InstanceButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick([NotNull] Window mainWindow)
    {
      if (!ApplicationManager.IsIisRunning)
      {
        string message = "The 'IIS' application is not running. Please start the app and re-run the Sitecore installation.";

        MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);

        return;
      }

      if (ApplicationManager.CurrentDockerStatus == ApplicationManager.DockerStatus.RunningWithWindowsContainers || 
        ApplicationManager.CurrentDockerStatus == ApplicationManager.DockerStatus.RunningWithLinuxContainers)
      {
        string urlToWikiPage = "https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Troubleshooting";

        string message = $@"The 'Docker Desktop' is running now. 
It may prevent the Sitecore installation process, due to the HTTPS port 443 usage conflict.
Please visit the '{urlToWikiPage}' for details.

Please stop the 'Docker Desktop' and continue the installation.

Do you want to proceed with the installation process?";

        if (MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
        {
          return;
        }
      }

      WizardPipelineManager.Start("install9", mainWindow, null, null, (args) =>
      {
        if (args == null)
        {
          return;
        }

        var install = (InstallWizardArgs)args;
        var product = install.Product;

        if (install.ShouldRefreshInstancesList)
        {
          MainWindowHelper.SoftlyRefreshInstances();
        }
        
      }, () => new Install9WizardArgs());
    }

    #endregion
  }
}

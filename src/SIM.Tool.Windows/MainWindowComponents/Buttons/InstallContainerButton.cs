namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class InstallContainerButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.IsTrue(ProfileManager.IsValid, "Some of configuration settings are invalid - please fix them in Settings dialog and try again");

      if (!ProductManager.ContainerProducts.Any())
      {
        string message =
$@"You don't have any container product package in your repository. 

If you already have them then you can either: 

* change the local repository folder (Ribbon -> Home -> Settings button) to the one that contains the files 

* download a package from 'https://github.com/Sitecore/container-deployment/releases', put the file into the current local repository folder: '{ProfileManager.Profile.LocalRepository}'";

        MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);

        return;
      }

      if (!ApplicationManager.IsDockerRunning)
      {
        string message = "The 'Docker Desktop' application is not running. Please start the app and re-run the deployment Sitecore to Docker.";

        MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);

        return;
      }

      if (EnvironmentHelper.CheckSqlServer())
      {
        WizardPipelineManager.Start("installContainer", mainWindow, null, null, (args) =>
        {

          if (args == null)
          {
            return;
          }

          var install = (InstallWizardArgs)args;
          var product = install.Product;
          if (product == null)
          {
            return;
          }

          if (install.ShouldRefreshInstancesList)
          {
            MainWindowHelper.RefreshInstances();
          }
        }, () => new InstallContainerWizardArgs());
      }
    }

    #endregion
  }
}
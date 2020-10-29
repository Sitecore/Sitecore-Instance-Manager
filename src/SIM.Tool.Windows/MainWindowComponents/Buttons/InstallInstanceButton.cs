using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class InstallInstanceButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
      Assert.IsTrue(ProfileManager.IsValid, "Some of configuration settings are invalid - please fix them in Settings dialog and try again");
      Assert.IsTrue(ProductManager.StandaloneProducts.Any(),
        $@"You don't have any standalone product package in your repository. Options to solve:

1. (recommended) Use Ribbon -> Home -> Bundled Tools -> Download Sitecores button to download them.

2. If you already have them then you can either: 

* change the local repository folder (Ribbon -> Home -> Settings button) to the one that contains the files 

* put the files into the current local repository folder: 
{ProfileManager.Profile.LocalRepository}");

      if (EnvironmentHelper.CheckSqlServer())
      {
        WizardPipelineManager.Start("install", mainWindow, null, null, (args) =>
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

          if (install.HasInstallationBeenCompleted)
          {
            MainWindowHelper.SoftlyRefreshInstances();
          }
        }, () => new InstallWizardArgs());
      }
    }

    #endregion
  }
}
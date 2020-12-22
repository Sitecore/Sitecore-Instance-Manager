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
      WizardPipelineManager.Start("install9", mainWindow, null, null, (args) =>
      {
        if (args == null)
        {
          return;
        }

        var install = (InstallWizardArgs)args;
        var product = install.Product;

        if (install.HasInstallationBeenCompleted)
        {
          MainWindowHelper.SoftlyRefreshInstances();
        }
        
      }, () => new Install9WizardArgs());
    }

    #endregion
  }
}

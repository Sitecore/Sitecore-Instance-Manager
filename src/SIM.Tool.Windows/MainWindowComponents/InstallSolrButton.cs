using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Wizards;
using System.Windows;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class InstallSolrButton : IMainWindowButton
  {
    public bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      return true;
    }

    public bool IsVisible([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      return true;
    }

    public void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      WizardPipelineManager.Start("installSolr", mainWindow, null, null, (args) =>
      {
        if (args == null)
        {
          return;
        }

        var install = (InstallWizardArgs)args;
        if (install.HasInstallationBeenCompleted)
        {
          MainWindowHelper.SoftlyRefreshInstances();
        }

      }, () => new Install9WizardArgs());
    }
  }
}

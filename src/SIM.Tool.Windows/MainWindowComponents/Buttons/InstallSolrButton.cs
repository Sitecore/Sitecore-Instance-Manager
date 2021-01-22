using System;
using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class InstallSolrButton : WindowOnlyButton
  {
    public event EventHandler<InstallWizardArgs> InstallationCompleted;

    #region Protected methods

    protected override void OnClick([NotNull] Window mainWindow)
    {
      this.InstallSolr(mainWindow);
    }

    #endregion

    #region Public methods

    public void InstallSolr(Window window, bool? isAsync = null)
    {
      WizardPipelineManager.Start("installSolr", window, null, isAsync, (args) =>
      {
        if (args == null)
        {
          return;
        }

        var install = (InstallWizardArgs)args;
        if (install.ShouldRefreshInstancesList)
        {
          MainWindowHelper.SoftlyRefreshInstances();
        }

        // Raise the event to refresh the list of Solr servers after installing the new one
        InstallationCompleted?.Invoke(this, install);
      }, () => new Install9WizardArgs());
    }

    #endregion
  }
}

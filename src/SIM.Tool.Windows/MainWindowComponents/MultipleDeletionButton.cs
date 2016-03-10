namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Collections.Generic;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.MultipleDeletion;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Windows;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class MultipleDeletionButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WizardPipelineManager.Start("multipleDeletion", mainWindow, new MultipleDeletionArgs(new List<string>()), null, OnWizardCompleted);
    }

    #endregion

    #region Private methods

    private static void OnWizardCompleted()
    {
      MainWindowHelper.SoftlyRefreshInstances();
    }

    #endregion
  }
}
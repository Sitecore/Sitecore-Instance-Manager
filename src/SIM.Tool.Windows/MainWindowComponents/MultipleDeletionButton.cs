namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Collections.Generic;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.MultipleDeletion;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Windows;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.MultipleDeletion;

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
      WizardPipelineManager.Start("multipleDeletion", mainWindow, new MultipleDeletionArgs(new List<string>()), null, ignore => OnWizardCompleted(), () => new MultipleDeletionWizardArgs());
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
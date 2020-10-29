using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using SIM.Pipelines.MultipleDeletion;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.MultipleDeletion;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class MultipleDeletionButton : WindowOnlyButton
  {
    #region Protected methods

    protected override void OnClick(Window mainWindow)
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
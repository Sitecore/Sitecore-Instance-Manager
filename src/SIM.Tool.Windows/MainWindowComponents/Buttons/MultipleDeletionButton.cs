using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
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
      if (InstanceManager.Default.Instances != null)
      {
        IOrderedEnumerable<Instance> instances = InstanceManager.Default.Instances.Where(instance => instance.Type == Instance.InstanceType.Sitecore8AndEarlier).OrderBy(instance => instance.Name);
        WizardPipelineManager.Start("multipleDeletion", mainWindow, new MultipleDeletionArgs(new List<string>()), null, wizardArgs => OnWizardCompleted(wizardArgs), () => new MultipleDeletionWizardArgs() { _Instances = instances });
      }
      else
      {
        if (MessageBox.Show("Sitecore instances cannot be loaded. Would you like to refresh Sitecore web sites and repeat the action?", "Multiple deletion Sitecore 8 and earlier", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          MainWindowHelper.RefreshInstances();
          this.OnClick(mainWindow);
        }
      }
    }

    #endregion

    #region Private methods

    private static void OnWizardCompleted(WizardArgs wizardArgs)
    {
      if (wizardArgs.ShouldRefreshInstancesList)
      {
        MainWindowHelper.SoftlyRefreshInstances();
      }
    }

    #endregion
  }
}
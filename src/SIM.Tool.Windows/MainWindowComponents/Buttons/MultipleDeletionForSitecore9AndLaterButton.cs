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
  public class MultipleDeletionForSitecore9AndLaterButton : WindowOnlyButton
  {
    protected override void OnClick(Window mainWindow)
    {
      if (InstanceManager.Default.Instances != null)
      {
        IOrderedEnumerable<Instance> instances = InstanceManager.Default.Instances.Where(instance => instance.Type == Instance.InstanceType.Sitecore9AndLater && instance.SitecoreEnvironment.Members != null).GroupBy(instance => instance.SitecoreEnvironment.Name).Select(instance => instance.First()).OrderBy(instance => instance.SitecoreEnvironment.Name);
        WizardPipelineManager.Start("multipleDeletion9", mainWindow, new MultipleDeletion9Args(new List<string>(), null), null, wizardArgs => OnWizardCompleted(wizardArgs), () => new MultipleDeletion9WizardArgs() { _Instances = instances, IsInstallationDetailsShown = true });
      }
      else
      {
        if (MessageBox.Show("Sitecore environments cannot be loaded. Would you like to refresh Sitecore web sites and repeat the action?", "Multiple deletion Sitecore 9 and later", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          MainWindowHelper.RefreshInstances();
          this.OnClick(mainWindow);
        }
      }
    }

    private static void OnWizardCompleted(WizardArgs wizardArgs)
    {
      if (wizardArgs.ShouldRefreshInstancesList)
      {
        MainWindowHelper.SoftlyRefreshInstances();
      }
    }
  }
}
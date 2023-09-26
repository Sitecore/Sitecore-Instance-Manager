using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.Windows;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class SearchAndDeleteResourcesButton : WindowOnlyButton
  {
    protected override void OnClick(Window mainWindow)
    {
      ResourcesWizardArgs resourcesWizardArgs = new ResourcesWizardArgs();
      WizardPipelineManager.Start("searchAndDeleteResources", mainWindow, new ProcessorArgs(), false, null, () => resourcesWizardArgs);
    }
  }
}
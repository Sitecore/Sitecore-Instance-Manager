using System.Collections.Generic;
using SIM.Pipelines.MultipleDeletion;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public class MultipleDeletionWizardArgs : WizardArgs
  {
    public List<string> SelectedInstances;

    public override ProcessorArgs ToProcessorArgs()
    {   
      return new MultipleDeletionArgs(SelectedInstances) {ConnectionString = ProfileManager.GetConnectionString()};
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using SIM.Instances;
using SIM.Pipelines.MultipleDeletion;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public class MultipleDeletion9WizardArgs : WizardArgs
  {
    public IOrderedEnumerable<Instance> _Instances;

    public List<string> _SelectedEnvironments;

    public bool _ScriptsOnly;

    public override ProcessorArgs ToProcessorArgs()
    {
      return new MultipleDeletion9Args(_SelectedEnvironments, this.Logger)
      {
        _ConnectionString = ProfileManager.GetConnectionString(),
        _ScriptsOnly = this._ScriptsOnly
      };
    }
  }
}
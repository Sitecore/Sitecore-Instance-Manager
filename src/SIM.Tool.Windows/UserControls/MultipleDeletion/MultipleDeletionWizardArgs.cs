namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Pipelines.MultipleDeletion;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class MultipleDeletionWizardArgs : WizardArgs
  {
    #region Fields

    public IOrderedEnumerable<Instance> _Instances;

    public List<string> _SelectedInstances;

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      return new MultipleDeletionArgs(_SelectedInstances)
      {
        _ConnectionString = ProfileManager.GetConnectionString()
      };
    }

    #endregion
  }
}
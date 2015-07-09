namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  using System.Collections.Generic;
  using SIM.Pipelines.MultipleDeletion;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class MultipleDeletionWizardArgs : WizardArgs
  {
    #region Fields

    public List<string> SelectedInstances;

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      return new MultipleDeletionArgs(this.SelectedInstances)
      {
        ConnectionString = ProfileManager.GetConnectionString()
      };
    }

    #endregion
  }
}
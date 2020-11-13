namespace SIM.Tool.Base.Pipelines
{
  using System.Data.SqlClient;

  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Wizards;

  public class InstallPublishingServiceWizardArgs : WizardArgs
  {
    public InstallPublishingServiceWizardArgs(Instance instance)
    {
      Instance = instance;
    }

    public Instance Instance { get; }
  }
}
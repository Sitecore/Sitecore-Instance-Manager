using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Base.Pipelines
{
  public class ResourcesWizardArgs : WizardArgs
  {
    public string InstanceName { get; set; }

    public string ConnectionString { get; set; }

    public string SolrUrl { get; set; }
  }
}
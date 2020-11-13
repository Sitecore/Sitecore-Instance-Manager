namespace SIM.Tool.Base.Pipelines
{
  using System.Collections.Generic;
  using System.Data.SqlClient;

  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using SIM.Services;
  using SIM.Tool.Base.Converters;
  using SIM.Tool.Base.Wizards;

  public class InstallPublishingServiceWizardArgs : WizardArgs
  {
    public InstallPublishingServiceWizardArgs(Instance instance)
    {
      Instance = instance;
      InstanceName = instance.Name;
      InstanceWebrootDirectory = instance.WebRootPath;

    }

    public Instance Instance { get; }
    public string InstanceName { get; }
    public string InstanceWebrootDirectory { get; }
    public string PublishingServiceName { get; set; }
    public string PublishingServiceWebrootDirectory { get; set; }
    public Dictionary<string, SqlConnectionStringBuilder> ConnectionStrings { get; set; }
  }
}
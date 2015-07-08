#region Usings

using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;
using SIM.Pipelines.Restore;
using SIM.Tool.Base.Wizards;

#endregion

namespace SIM.Tool.Base.Pipelines
{
  public class RestoreWizardArgs : WizardArgs
  {
    public readonly Instance Instance;
    private readonly string instanceName;

    public RestoreWizardArgs(Instance instance)
    {
      Instance = instance;
      instanceName = instance.Name;
    }

    public InstanceBackup Backup { get; set; }

    public string InstanceName
    {
      get { return this.instanceName; }
    }

    public override ProcessorArgs ToProcessorArgs()
    {
      Assert.IsNotNull(Backup, "Any backup wasn't chosen", false);
      return new RestoreArgs(Instance, Backup);
    }
  }
}

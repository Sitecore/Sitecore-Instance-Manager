using System;
using SIM.Tool.Base.Wizards;
using SIM.ContainerInstaller;
using SIM.Pipelines.Processors;
using SIM.Pipelines.Delete.Containers;

namespace SIM.Tool.Base.Pipelines
{
  public class DeleteContainersWizardArgs : WizardArgs
  {
    public string DestinationFolder { get; set; }

    public Guid EnvironmentId { get; set; }

    public EnvModel Env { get; set; }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new DeleteContainersArgs(
        this.DestinationFolder,
        this.Env,
        this.EnvironmentId
        );
    }
  }
}

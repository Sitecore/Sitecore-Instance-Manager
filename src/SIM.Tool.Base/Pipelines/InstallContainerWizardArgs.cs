using SIM.ContainerInstaller;
using SIM.Instances;
using SIM.Pipelines.Install.Containers;
using SIM.Pipelines.Processors;

namespace SIM.Tool.Base.Pipelines
{
  public class InstallContainerWizardArgs : InstallWizardArgs
  {
    public InstallContainerWizardArgs() : base()
    {
    }

    public InstallContainerWizardArgs(Instance instance) : base(instance)
    {
    }

    public string FilesRoot { get; set; }
    public string Tag { get; set; }
    public string DockerRoot { get; set; }
    public string DestinationFolder { get; set; }
    public EnvModel EnvModel { get; set; }
    public bool ScriptsOnly { get; set; }
    public string Topology { get; set; }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new InstallContainerArgs(
        this.EnvModel,
        this.DestinationFolder,
        this.DockerRoot,
        this.Topology,
        this.Logger,
        this.ScriptsOnly
        );
    }
  }
}

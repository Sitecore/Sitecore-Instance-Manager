using SIM.Instances;
using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Base.Pipelines
{
  public class InstallContainerWizardArgs:InstallWizardArgs
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

    public override ProcessorArgs ToProcessorArgs()
    {
      throw new NotImplementedException("to do: create new pipeline args type");
    }
  }
}

using ContainerInstaller;
using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install.Containers
{
  public class InstallContainerArgs:ProcessorArgs
  {
    public InstallContainerArgs(EnvModel model, string destination, string filesRoot)
    {
      this.EnvModel = model;
      this.FilesRoot = filesRoot;
      this.Destination = destination;
    }
    public EnvModel EnvModel { get; }
    public string Destination
    {
      get; set;
    }

    public string FilesRoot
    {
      get;
    }
  }
}

using System;
using SIM.ContainerInstaller;
using SIM.Loggers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  public enum Topology
  {
    Xm1,
    Xp0,
    Xp1
  }

  public class InstallContainerArgs : ProcessorArgs
  {
    public InstallContainerArgs(EnvModel model, string destination, string filesRoot, string topology, ILogger logger)
    {
      this.EnvModel = model;
      this.FilesRoot = filesRoot;
      this.Destination = destination;
      this.Topology = (Topology)Enum.Parse(typeof(Topology), topology, true);
      this.Logger = logger;
    }

    public EnvModel EnvModel { get; }

    public string Destination
    {
      get; set;
    }

    public Topology Topology { get; }

    public string FilesRoot
    {
      get;
    }

    public ILogger Logger
    {
      get; set;
    }
  }
}

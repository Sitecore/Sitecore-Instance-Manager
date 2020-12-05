using System;
using System.Globalization;
using ContainerInstaller;
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

    public InstallContainerArgs(EnvModel model, string destination, string filesRoot, string topology)
    {
      this.EnvModel = model;
      this.FilesRoot = filesRoot;
      this.Destination = destination;
      this.Topology = (Topology)Enum.Parse(typeof(Topology), topology, true);
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
  }
}

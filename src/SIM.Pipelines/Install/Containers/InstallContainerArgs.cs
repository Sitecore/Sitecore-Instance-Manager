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

  public static class StringExtentions
  {
    public static Topology ToTopology(this string tpl)
    {
      if (tpl.Equals("xp0", StringComparison.InvariantCultureIgnoreCase))
      {
        return Topology.Xp0;
      }

      if (tpl.Equals("xp1", StringComparison.InvariantCultureIgnoreCase))
      {
        return Topology.Xp1;
      }

      if (tpl.Equals("xm1", StringComparison.InvariantCultureIgnoreCase))
      {
        return Topology.Xm1;
      }
      
      throw new InvalidOperationException("Topology cannot be resolved from '" + tpl + "'");
    }
  }

  public class InstallContainerArgs : ProcessorArgs
  {

    public InstallContainerArgs(EnvModel model, string destination, string filesRoot, string topology)
    {
      this.EnvModel = model;
      this.FilesRoot = filesRoot;
      this.Destination = destination;
      this.Topology = topology.ToTopology();
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

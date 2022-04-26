using System;
using System.Collections.Generic;
using System.ComponentModel;
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

  public enum Module
  {
    [Description("SXA")]
    SXA,
    [Description("JSS")]
    JSS,
    [Description("Horizon")]
    Horizon,
    [Description("Publishing Service")]
    PublishingService
  }

  public class InstallContainerArgs : ProcessorArgs
  {
    public InstallContainerArgs(
      EnvModel model,
      string destination,
      string filesRoot,
      string topology,
      ILogger logger,
      bool scriptsOnly,
      IEnumerable<Module> modules
      )
    {
      this.EnvModel = model;
      this.FilesRoot = filesRoot;
      this.Destination = destination;
      this.Topology = (Topology)Enum.Parse(typeof(Topology), topology, true);
      this.Logger = logger;
      this.ScriptsOnly = scriptsOnly;
      this.Modules = modules;
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

    public bool ScriptsOnly { get; }

    public IEnumerable<Module> Modules
    {
      get; set;
    }
  }
}

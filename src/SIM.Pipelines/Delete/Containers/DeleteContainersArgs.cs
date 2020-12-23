using System;
using SIM.ContainerInstaller;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Delete.Containers
{
  public class DeleteContainersArgs : ProcessorArgs
  {
    public string DestinationFolder { get; }

    public EnvModel Env { get; }

    public Guid EnvironmentId { get; }

    public DeleteContainersArgs(string destinationFolder, EnvModel env, Guid environmentId)
    {
      Assert.ArgumentNotNullOrEmpty(destinationFolder, "destinationFolder");
      Assert.ArgumentNotNull(env, "env");

      this.DestinationFolder = destinationFolder;
      this.Env = env;
      this.EnvironmentId = environmentId;
    }
  }
}

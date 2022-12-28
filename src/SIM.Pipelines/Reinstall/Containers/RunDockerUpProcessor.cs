using JetBrains.Annotations;
using SIM.Loggers;
using SIM.Pipelines.Delete.Containers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Reinstall.Containers
{
  [UsedImplicitly]
  public class RunDockerUpProcessor : Install.Containers.RunDockerUpProcessor
  {
    protected override string GetExecutionFolder(ProcessorArgs procArgs)
    {
      DeleteContainersArgs args = (DeleteContainersArgs)procArgs;

      return args.DestinationFolder;
    }

    protected override ILogger GetLogger(ProcessorArgs procArgs)
    {
      DeleteContainersArgs args = (DeleteContainersArgs)procArgs;

      return args.Logger;
    }
  }
}
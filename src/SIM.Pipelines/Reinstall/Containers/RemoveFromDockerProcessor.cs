using JetBrains.Annotations;
using SIM.Loggers;
using SIM.Pipelines.BaseProcessors;
using SIM.Pipelines.Delete.Containers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Reinstall.Containers
{
  [UsedImplicitly]
  public class RemoveFromDockerProcessor : RunCmdCommandBaseProcessor
  {
    protected override string GetCommand(ProcessorArgs procArgs)
    {
      return "docker-compose.exe down";
    }

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
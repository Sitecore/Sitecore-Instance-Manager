using SIM.Loggers;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Pipelines.BaseProcessors;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class RunDockerProcessor : RunCmdCommandBaseProcessor
  {
    protected override string GetCommand(ProcessorArgs procArgs)
    {
      return "docker-compose.exe up -d";
    }

    protected override string GetExecutionFolder(ProcessorArgs procArgs)
    {
      InstallContainerArgs args = (InstallContainerArgs)procArgs;

      return args.Destination;
    }

    protected override ILogger GetLogger(ProcessorArgs procArgs)
    {
      InstallContainerArgs args = (InstallContainerArgs)procArgs;

      return args.Logger;
    }
  }
}
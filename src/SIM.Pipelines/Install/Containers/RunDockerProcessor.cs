using SIM.Loggers;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Pipelines.BaseProcessors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class RunDockerProcessor : RunCmdCommandBaseProcessor
  {
    protected override string GetCommand(ProcessorArgs procArgs)
    {
      return "docker-compose.exe build --no-cache && docker-compose.exe up -d";
    }

    protected override string GetExecutionFolder(ProcessorArgs procArgs)
    {
      InstallContainerArgs args = (InstallContainerArgs)procArgs;

      return args.Destination;
    }

    protected override ILogger GetLogger(ProcessorArgs procArgs)
    {
      InstallContainerArgs args = (InstallContainerArgs)procArgs;

      Assert.IsNotNull(args, nameof(args));

      return args.Logger;
    }

    public override bool IsRequireProcessing([NotNull] ProcessorArgs procArgs)
    {
      InstallContainerArgs args = (InstallContainerArgs)procArgs;

      Assert.IsNotNull(args, nameof(args));

      return !args.ScriptsOnly;
    }
  }
}
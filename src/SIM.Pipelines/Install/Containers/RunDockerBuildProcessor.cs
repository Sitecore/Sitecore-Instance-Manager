using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class RunDockerBuildProcessor : RunDockerUpProcessor
  {
    protected override string GetCommand(ProcessorArgs procArgs)
    {
      return "docker-compose.exe build --no-cache";
    }
  }
}
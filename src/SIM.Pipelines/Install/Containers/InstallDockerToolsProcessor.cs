using System.IO;
using SIM.ContainerInstaller;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class InstallDockerToolsProcessor : Processor
  {
    protected override void Process(ProcessorArgs args)
    {
      string scriptFile = Path.Combine(Directory.GetCurrentDirectory(), "ContainerFiles/scripts/InstallDockerToolsModuleScript.txt");
      PSFileExecutor ps = new PSFileExecutor(scriptFile, Directory.GetCurrentDirectory());

      ps.Execute();
    }
  }
}

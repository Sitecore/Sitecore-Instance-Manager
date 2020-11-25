using System.IO;
using ContainerInstaller;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class InstallDockerToolsProcessor : Processor
  {
    protected override void Process(ProcessorArgs args)
    {
      string scriptFile = Path.Combine(Directory.GetCurrentDirectory(), "ContainerFiles/scripts/InstallDockerToolsModuleScript.ps1");
      PSFileExecutor ps = new PSFileExecutor(scriptFile, Directory.GetCurrentDirectory());

      ps.Execute();
    }
  }
}

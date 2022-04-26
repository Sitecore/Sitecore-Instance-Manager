using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SIM.ContainerInstaller;
using SIM.ContainerInstaller.Modules;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  public class GenerateModulesData : Processor
  {
    private readonly OverrideYamlFileGenerator overrideYamlFileGenerator = new OverrideYamlFileGenerator();

    private readonly DockerfileGenerator dockerfileGenerator = new DockerfileGenerator();

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, nameof(arguments));

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      Assert.ArgumentNotNull(args, nameof(args));

      foreach (Module module in args.Modules)
      {
        switch (module)
        {
          case Module.SXA:
            SXAHelper sxaHelper = new SXAHelper();
            overrideYamlFileGenerator.Generate(args.Destination, sxaHelper);
            dockerfileGenerator.Generate(args.Destination, sxaHelper);
            break;
          case Module.JSS:
            break;
          case Module.Horizon:
            break;
          case Module.PublishingService:
            break;
          default:
            break;
        }
      }
    }
  }
}
using System.Collections.Generic;
using JetBrains.Annotations;
using SIM.ContainerInstaller;
using SIM.ContainerInstaller.Modules;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  public class GenerateModulesData : Processor
  {
    private List<IYamlFileGeneratorHelper> yamlFileGeneratorHelpers;

    private List<IDockerfileGeneratorHelper> dockerfileGeneratorHelpers;

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, nameof(arguments));

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      Assert.ArgumentNotNull(args, nameof(args));

      if (args.Modules != null && args.Modules.Count > 0)
      {
        string topology = args.Topology.ToString().ToLower();

        yamlFileGeneratorHelpers = new List<IYamlFileGeneratorHelper>();
        dockerfileGeneratorHelpers = new List<IDockerfileGeneratorHelper>();

        yamlFileGeneratorHelpers.Add(new ToolsYamlFileGeneratorHelper());
        dockerfileGeneratorHelpers.Add(new ToolsDockerfileGeneratorHelper());

        foreach (Module module in args.Modules)
        {
          switch (module)
          {
            case Module.SXA:
              yamlFileGeneratorHelpers.Add(new SxaYamlFileGeneratorHelper(topology));
              dockerfileGeneratorHelpers.Add(new SxaDockerfileGeneratorHelper());
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

        new YamlFileGenerator(topology).Generate(args.Destination, yamlFileGeneratorHelpers, args.VersionAndTopology);
        new DockerfileGenerator().Generate(args.Destination, dockerfileGeneratorHelpers, args.VersionAndTopology);
      }
    }
  }
}
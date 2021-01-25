using SIM.ContainerInstaller;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class GenerateTelerikKeyProcessor : Processor
  {
    private readonly IKeyGenerator _generator = new TelerikKeyGenerator();
    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      args.EnvModel.TelerikKey = this._generator.Generate();
    }
  }
}
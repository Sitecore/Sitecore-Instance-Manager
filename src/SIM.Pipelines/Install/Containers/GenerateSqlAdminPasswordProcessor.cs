using JetBrains.Annotations;
using SIM.ContainerInstaller;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class GenerateSqlAdminPasswordProcessor : Processor
  {
    private readonly IGenerator _generator = new SqlAdminPasswordGenerator();
    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      if (string.IsNullOrEmpty(args.EnvModel.SqlAdminPassword))
      {
        args.EnvModel.SqlAdminPassword = this._generator.Generate();
      }
    }
  }
}

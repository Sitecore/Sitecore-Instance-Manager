using JetBrains.Annotations;
using SIM.Pipelines.Install.Containers;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Delete.Containers
{
  [UsedImplicitly]
  public class RemoveEnvironmentFolder : Processor
  {
    protected override void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      DeleteContainersArgs deleteArgs = (DeleteContainersArgs)args;
      Assert.ArgumentNotNull(deleteArgs, "deleteArgs");

      FileSystem.FileSystem.Local.Directory.DeleteIfExists(deleteArgs.DestinationFolder);
    }
  }
}

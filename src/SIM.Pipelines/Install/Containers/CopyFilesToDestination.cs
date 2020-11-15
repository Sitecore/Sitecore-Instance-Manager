using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  public class CopyFilesToDestination : Processor
  {
    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      InstallContainerArgs args = (InstallContainerArgs)arguments;
      SIM.FileSystem.FileSystem.Local.Directory.Copy(args.FilesRoot, args.Destination, true);
    }
  }
}

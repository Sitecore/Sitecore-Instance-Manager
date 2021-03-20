using System.IO.Compression;

namespace SIM.Pipelines.InstallPublishingService
{
  public class PrepareInstallationProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      ZipFile.ExtractToDirectory(args.SPSPackagePath, args.SPSWebroot);
      Commands.CommandRoot = args.SPSWebroot;
    }
  }
}

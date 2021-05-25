using System.IO.Compression;

namespace SIM.Pipelines.PublishingService.Install
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

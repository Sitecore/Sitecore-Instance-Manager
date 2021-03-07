using System.IO.Compression;

namespace SIM.Pipelines.InstallPublishingService
{
  public class UnzipSPSPackageProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      ZipFile.ExtractToDirectory(args.SPSPackagePath, args.SPSWebroot);
    }
  }
}

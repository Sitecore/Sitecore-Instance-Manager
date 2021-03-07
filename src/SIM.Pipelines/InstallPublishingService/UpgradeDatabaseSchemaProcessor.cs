using System.IO;

namespace SIM.Pipelines.InstallPublishingService
{
  public class UpgradeDatabaseSchemaProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.SPSWebroot);
      Commands.SchemaUpgrade();
    }
  }
}

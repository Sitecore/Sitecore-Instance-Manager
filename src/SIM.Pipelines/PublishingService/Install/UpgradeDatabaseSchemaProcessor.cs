namespace SIM.Pipelines.PublishingService.Install
{
  public class UpgradeDatabaseSchemaProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Commands.SchemaUpgrade();
    }
  }
}

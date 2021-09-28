namespace SIM.Pipelines.PublishingService.Install
{
  public class ResetDatabaseSchemaProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    //Resetting the schema is done to clear any potentially existing Publishing tables 
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Commands.SchemaReset();
    }
  }
}

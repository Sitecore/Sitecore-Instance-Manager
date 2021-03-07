using System.IO;

namespace SIM.Pipelines.InstallPublishingService
{
  public class ResetDatabaseSchemaProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    //Resetting the schema is done to clear any potentially existing Publishing tables 
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.SPSWebroot);
      Commands.SchemaReset();
    }
  }
}

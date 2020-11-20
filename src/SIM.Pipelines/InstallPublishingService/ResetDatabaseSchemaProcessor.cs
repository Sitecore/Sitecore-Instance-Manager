using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class ResetDatabaseSchemaProcessor : InstallPublishingServiceProcessor
  {
    //Resetting the schema is done to clear any potentially existing Publishing tables 
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.PubilshingServiceWebroot);
      Commands.SchemaReset();
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class UpgradeDatabaseSchemaProcessor : InstallPublishingServiceProcessor
  {
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.PubilshingServiceWebroot);
      Commands.SchemaUpgrade();
    }
  }
}

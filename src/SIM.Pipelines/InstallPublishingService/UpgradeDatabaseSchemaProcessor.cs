using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class UpgradeDatabaseSchemaProcessor : InstallSPSProcessor
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.SPSWebroot);
      Commands.SchemaUpgrade();
    }
  }
}

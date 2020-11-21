using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class CreateIISiteProcessor : InstallSPSProcessor
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Commands.IISInstall(args.SPSSiteName, args.SPSSiteName);
    }
  }
}

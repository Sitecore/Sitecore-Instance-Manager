using JetBrains.Annotations;
using Microsoft.Web.Administration;
using SIM.Adapters.WebServer;
using SIM.Pipelines.UninstallPublishingService;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class RemoveExistingSPSProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      if (!args.OverwriteExisting)
      {
        return;
      }

      UninstallSPSProcessorArgs uninstallArgs = new UninstallSPSProcessorArgs()
      {
        SPSSiteName = args.SPSSiteName,
        SPSAppPoolName = args.SPSAppPoolName,
        SPSWebroot = args.SPSWebroot,
        SkipSPSSite = false,
        SkipSPSAppPool = false,
        SkipSPSWebroot = false
      };

      new RemoveIISSiteProcessor().DoProcess(uninstallArgs);
      new RemoveAppPoolProcessor().DoProcess(uninstallArgs);
      new RemoveWebrootFolderProcessor().DoProcess(uninstallArgs);
    }
  }
}

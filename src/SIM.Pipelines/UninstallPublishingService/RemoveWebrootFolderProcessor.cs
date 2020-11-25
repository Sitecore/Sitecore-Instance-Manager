using JetBrains.Annotations;
using SIM.Pipelines.InstallPublishingService;
using System;
using System.IO;

namespace SIM.Pipelines.UninstallPublishingService
{
  public class RemoveWebrootFolderProcessor : SPSProcessor<UninstallSPSProcessorArgs>
  {
    protected override void ProcessCore([NotNull] UninstallSPSProcessorArgs args)
    {
      if (args.SkipSPSWebroot)
      {
        return;
      }

      if (Directory.Exists(args.SPSWebroot))
      {
        try
        {
          Directory.Delete(args.SPSWebroot, true);
        }
        catch (IOException ex)
        {
          //Unlikely user scenario, but it can occur when you create an instance, then immediately try to overwrite it without restarting SIM
          throw new Exception($"SIM may be locking the {args.SPSWebroot} folder if it was just created.  Try restarting SIM and installing publishing service again", ex);
        }
      }
    }
  }
}

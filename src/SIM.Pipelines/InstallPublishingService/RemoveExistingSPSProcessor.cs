using SIM.Pipelines.UninstallPublishingService;

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

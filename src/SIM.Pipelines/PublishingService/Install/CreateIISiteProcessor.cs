namespace SIM.Pipelines.PublishingService.Install
{
  public class CreateIISiteProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Commands.IISInstall(args.SPSSiteName, args.SPSSiteName, args.SPSPort);
    }
  }
}

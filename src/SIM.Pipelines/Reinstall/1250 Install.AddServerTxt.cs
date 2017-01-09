namespace SIM.Pipelines.Reinstall
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public class AddServerTxt : ReinstallProcessor
  {
    [UsedImplicitly]
    public AddServerTxt()
    {
    }
      
    protected override void Process(ReinstallArgs args)
    {
      File.WriteAllText(Path.Combine(args.WebRootPath, "server.txt"), $"{Environment.MachineName}-{args.InstanceName}");
    }
  }
}

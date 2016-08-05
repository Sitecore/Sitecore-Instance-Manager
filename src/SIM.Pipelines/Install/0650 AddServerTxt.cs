namespace SIM.Pipelines.Install
{
  using System;
  using System.IO;
  using Sitecore.Diagnostics.Base.Annotations;

  public class AddServerTxt : InstallProcessor
  {
    [UsedImplicitly]
    public AddServerTxt()
    {
    }

    protected override void Process(InstallArgs args)
    {
      File.WriteAllText(Path.Combine(args.WebRootPath, "server.txt"), $"{Environment.MachineName}-{args.InstanceName}");
    }
  }
}

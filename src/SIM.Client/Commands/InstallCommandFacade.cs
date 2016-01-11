namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Core.Commands;
  using Sitecore.Diagnostics.Base.Annotations;

  public class InstallCommandFacade : InstallCommand
  {
    [UsedImplicitly]
    public InstallCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('p', "product")]
    public override string Product { get; set; }

    [Option('v', "version")]
    public override string Version { get; set; }

    [Option('r', "revision")]
    public override string Revision { get; set; }
  }
}
namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  public class SyncCommandFacade : SyncCommand
  {
    [UsedImplicitly]
    public SyncCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('s', "source", Required = true, HelpText = "Source instance name.")]
    public override string Name { get; set; }

    [Option('t', "targets", Required = true, HelpText = "Pipe-separated list of target instances names")]
    public override string Targets { get; set; }

    [Option('i', "ignore", Required = false, HelpText = "Pipe-separated list of ignore patterns")]
    public override string Ignore { get; set; }
  }
}
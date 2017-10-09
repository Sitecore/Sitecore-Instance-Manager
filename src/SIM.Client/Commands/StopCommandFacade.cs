namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("stop", HelpText = "Stop an instance.")]
  public class StopCommandFacade : StopCommand
  {
    [UsedImplicitly]
    public StopCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('f', "force")]
    public override bool Force { get; set; }
  }
}
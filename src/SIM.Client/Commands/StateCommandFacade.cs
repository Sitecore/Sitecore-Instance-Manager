namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("state", HelpText = "Show state of an instance.")]
  public class StateCommandFacade : StateCommand
  {
    [UsedImplicitly]
    public StateCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
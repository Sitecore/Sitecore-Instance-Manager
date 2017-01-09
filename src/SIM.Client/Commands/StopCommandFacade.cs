namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;

  public class StopCommandFacade : StopCommand
  {
    [UsedImplicitly]
    public StopCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('f', "force")]
    public override bool? Force { get; set; }
  }
}
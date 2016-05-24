namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Core.Commands;

  public class StartCommandFacade : StartCommand
  {
    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}

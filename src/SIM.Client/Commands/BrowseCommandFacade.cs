namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Core.Commands;

  public class BrowseCommandFacade : BrowseCommand
  {
    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
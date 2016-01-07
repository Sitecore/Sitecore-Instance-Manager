namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Commands;

  public class ListProjectsCommandFacade : ListProjectsCommand
  {
    [Option('f', "filter", HelpText = "Tell the command to automatically stage files.")]
    public override string Filter { get; set; }
  }
}

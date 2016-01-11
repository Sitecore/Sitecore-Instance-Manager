namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Core.Commands;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListCommandFacade : ListCommand
  {
    [UsedImplicitly]
    public ListCommandFacade()
    {
    }

    [Option('f', "filter", HelpText = "Show only instances that have specific string in the name.")]
    public override string Filter { get; set; }

    [Option('e', "everywhere", HelpText = "When specified, shows instances that are located both within and without instances root folder.")]
    public override bool Everywhere { get; set; }
  }
}

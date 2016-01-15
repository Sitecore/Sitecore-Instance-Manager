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

    [Option('d', "detailed", HelpText = "When specified, extra information is collected, computed and reported.")]
    public override bool Detailed { get; set; }

    [Option('f', "filter", HelpText = "Show only instances that have specific string in the name.")]
    public override string Filter { get; set; }

    [Option('e', "everywhere", HelpText = "When specified, shows instances that are located both within and without instances root folder.")]
    public override bool Everywhere { get; set; }
  }
}

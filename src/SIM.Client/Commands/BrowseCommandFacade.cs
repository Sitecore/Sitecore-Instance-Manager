namespace SIM.Client.Commands
{
  using CommandLine;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Commands;

  public class BrowseCommandFacade : BrowseCommand
  {
    [UsedImplicitly]
    public BrowseCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
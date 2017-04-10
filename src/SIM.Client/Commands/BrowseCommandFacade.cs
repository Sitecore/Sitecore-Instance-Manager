namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  public class BrowseCommandFacade : BrowseCommand
  {
    [UsedImplicitly]
    public BrowseCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
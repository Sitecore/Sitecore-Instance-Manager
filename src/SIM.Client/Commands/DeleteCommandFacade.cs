namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Core.Commands;
  using Sitecore.Diagnostics.Base.Annotations;

  public class DeleteCommandFacade : DeleteCommand
  {
    [UsedImplicitly]
    public DeleteCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
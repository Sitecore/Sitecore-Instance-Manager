namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;

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
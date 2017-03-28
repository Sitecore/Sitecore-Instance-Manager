namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;

  public class StartCommandFacade : StartCommand
  {
    [UsedImplicitly]
    public StartCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
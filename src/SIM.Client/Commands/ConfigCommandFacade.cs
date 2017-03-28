namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;

  public class ConfigCommandFacade : ConfigCommand
  {
    [UsedImplicitly]
    public ConfigCommandFacade()
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('d', "database", Required = true)]
    public override string Database { get; set; }
  }
}
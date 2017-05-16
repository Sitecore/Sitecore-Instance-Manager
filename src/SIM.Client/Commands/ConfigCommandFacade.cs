namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("config", HelpText = "Show config of an instance.")]
  public class ConfigCommandFacade : ConfigCommand
  {
    [UsedImplicitly]
    public ConfigCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('d', "database", Required = true)]
    public override string Database { get; set; }
  }
}
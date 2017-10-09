namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("start", HelpText = "Start stopped instance.")]
  public class StartCommandFacade : StartCommand
  {
    [UsedImplicitly]
    public StartCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }
  }
}
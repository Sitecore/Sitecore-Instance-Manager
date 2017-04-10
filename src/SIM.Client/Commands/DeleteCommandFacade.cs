
namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  public class DeleteCommandFacade : DeleteCommand
  {
    [UsedImplicitly]
    public DeleteCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true, HelpText = "Name or pipe-separated list of IIS website names to delete")]
    public override string Name { get; set; }
  }
}
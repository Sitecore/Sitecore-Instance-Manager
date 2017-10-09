namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("repository", HelpText = "Show contents of repository.")]
  public class RepositoryCommandFacade : RepositoryCommand
  {
    [UsedImplicitly]
    public RepositoryCommandFacade()
      : base(new RealFileSystem())
    {
    }
  }
}
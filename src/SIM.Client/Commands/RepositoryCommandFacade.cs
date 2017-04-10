namespace SIM.Client.Commands
{
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  public class RepositoryCommandFacade : RepositoryCommand
  {
    [UsedImplicitly]
    public RepositoryCommandFacade()
      : base(new RealFileSystem())
    {
    }
  }
}
namespace SIM.Core.Commands
{
  using System.Collections.Generic;

  public class RepositoryCommandResult
  {
    public IReadOnlyCollection<string> Standalone { get; set; }

    public IReadOnlyCollection<string> Modules { get; set; }
  }
}
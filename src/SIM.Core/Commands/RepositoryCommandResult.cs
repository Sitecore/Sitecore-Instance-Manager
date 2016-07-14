namespace SIM.Core.Commands
{
  using System.Collections.Generic;
  using SIM.Core.Common;

  public class RepositoryCommandResult : CommandResult
  {
    public IReadOnlyCollection<string> Standalone { get; set; }

    public IReadOnlyCollection<string> Modules { get; set; }
  }
}
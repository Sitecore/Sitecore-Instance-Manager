namespace SIM.Core.Common
{
  using System;

  public class CommandResult : CommandResultBase
  {
    public TimeSpan Elapsed { get; set; }
  }
}
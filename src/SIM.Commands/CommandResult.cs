namespace SIM.Commands
{
  using System;

  public class CommandResult : CommandResultBase
  {
    public TimeSpan Elapsed { get; set; }
  }
}
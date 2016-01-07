namespace SIM.Commands.Common
{
  using System;

  public class CommandResult : CommandResultBase
  {
    public TimeSpan Elapsed { get; set; }
  }
}
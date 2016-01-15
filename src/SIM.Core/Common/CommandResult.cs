namespace SIM.Core.Common
{
  using System;

  public class CommandResult<TResult> : CommandResultBase<TResult>
  {
    public TimeSpan Elapsed { get; set; }
  }
}
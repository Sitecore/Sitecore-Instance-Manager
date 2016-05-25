namespace SIM.Core.Common
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base.Annotations;

  public class CommandResult<TResult> : CommandResult
  {
    public TResult Data { get; set; }
  }

  public abstract class CommandResult
  {
    public bool Success { get; set; }

    [CanBeNull]
    public string Message { get; set; }

    public TimeSpan Elapsed { get; set; }

    public CustomException Error { get; set; }
  }
}
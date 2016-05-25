namespace SIM.Core.Common
{
  using System;
  using Sitecore.Diagnostics.Base.Annotations;

  public class CommandResult<TResult> : CommandResult
  {
    /// <summary>
    /// Optional command output data.
    /// </summary>
    [CanBeNull]
    public TResult Data { get; set; }
  }

  public abstract class CommandResult
  {
    /// <summary>
    /// Command success indicator.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Optional message from command execution for comments clarifying situation when Success = false.
    /// </summary>
    [CanBeNull]
    public string Message { get; set; }

    /// <summary>
    /// Command pure execution time.
    /// </summary>
    public TimeSpan Elapsed { get; set; }

    /// <summary>
    /// Exception details in JSON-friendly format.
    /// </summary>
    [CanBeNull]
    public CustomException Error { get; set; }
  }
}
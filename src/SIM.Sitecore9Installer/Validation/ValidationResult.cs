using System;

namespace SIM.Sitecore9Installer.Validation
{
  public enum ValidatorState
  {
    Pending,
    Error,
    Warning,
    Success
  };

  public class ValidationResult
  {
    public ValidationResult(ValidatorState state, string message, Exception exception)
    {
      this.State = state;
      this.Message = message;
      this.Exception = exception;
    }

    [RenderInDataGreed]
    public ValidatorState State { get; set; }

    [RenderInDataGreed]
    public string Message { get; set; }

    [RenderInDataGreed]
    public Exception Exception { get; set; }
  }
}

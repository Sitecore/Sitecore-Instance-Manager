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
    public ValidationResult(ValidatorState state, string message, Exception error)
    {
      this.State = state;
      this.Message = message;
      this.Error = error;
    }

    [RenderInDataGreed]
    public ValidatorState State { get; set; }

    [RenderInDataGreed]
    public string Message { get; set; }

    [RenderInDataGreed]
    public Exception Error { get; set; }
  }
}

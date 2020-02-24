using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public ValidatorState State { get; }
    public string Message { get; }
    public Exception Error { get; }
  }
}

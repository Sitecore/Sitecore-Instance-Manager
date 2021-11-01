using System.Collections.Generic;
using System.IO;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class PathExistsValidator : BaseValidator
  {
    public override string SuccessMessage => null;

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Tasks.Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach(InstallParam p in paramsToValidate)
      {
        if (!this.PathExists(p.Value))
        {
          yield return new ValidationResult(ValidatorState.Error, $"The following path does not exist:\n{p.Value}", null);
        }
        else
        {
          yield return new ValidationResult(ValidatorState.Success, $"The following path exists:\n{p.Value}", null);
        }
      }
    }

    protected internal virtual bool PathExists(string path)
    {
      return Directory.Exists(path);
    }
  }
}

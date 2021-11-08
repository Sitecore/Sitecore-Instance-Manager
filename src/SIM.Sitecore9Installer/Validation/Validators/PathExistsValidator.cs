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
          yield return new ValidationResult(ValidatorState.Error, $"The '{p.Value}' path defined in the '{p.Name}' parameter of the '{task.Name}' installation task does not exist.", null);
        }
        else
        {
          yield return new ValidationResult(ValidatorState.Success, $"The '{p.Value}' path defined in the '{p.Name}' parameter of the '{task.Name}' installation task exists.", null);
        }
      }
    }

    protected internal virtual bool PathExists(string path)
    {
      return Directory.Exists(path);
    }
  }
}

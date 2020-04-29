using System.Collections.Generic;
using System.IO;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class PathExistsValidator : BaseValidator
  {
    protected override IEnumerable<ValidationResult> GetErrorsForTask(Tasks.Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach(InstallParam p in paramsToValidate)
      {
        if (!this.PathExists(p.Value))
        {
          yield return new ValidationResult(ValidatorState.Error, $"Path {p.Value} does not exist.", null);
        }
      }
    }

    protected internal virtual bool PathExists(string path)
    {
      return Directory.Exists(path);
    }
  }
}

using System;
using System.Collections.Generic;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostNameValidator : BaseValidator
  {
    public override string SuccessMessage => "'Uri.CheckHostName()' validation has been finished successfully.";
    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        if (Uri.CheckHostName(param.Value) != UriHostNameType.Dns)
        {
          ValidationResult r = new ValidationResult(ValidatorState.Error,
            string.Format("Invalid host in '{0}' of '{1}'", param.Name, task.Name), null);
          yield return r;
        }
      }
    }
  }
}

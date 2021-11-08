using System;
using System.Collections.Generic;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostNameValidator : BaseValidator
  {
    public override string SuccessMessage => "Hosts defined in parameters of installations tasks are valid DNS names.";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        if (Uri.CheckHostName(param.Value) != UriHostNameType.Dns)
        {
          ValidationResult r = new ValidationResult(ValidatorState.Error,
            string.Format("Invalid host is defined in the '{0}' parameter of the '{1}' installation task.", param.Name, task.Name), null);
          yield return r;
        }
      }
    }
  }
}

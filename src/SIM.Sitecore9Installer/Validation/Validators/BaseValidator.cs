using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public abstract class BaseValidator : IValidator
  {
    public abstract string SuccessMessage { get; }

    public BaseValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public virtual IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      List<ValidationResult> results = new List<ValidationResult>();
      foreach (Task task in tasks)
      {
        IEnumerable<InstallParam> tParams = task.LocalParams.Where(p => !this.GetParamNames().Any() || this.GetParamNames().Contains(p.Name));
        if (tParams.Any() || !task.LocalParams.Any())
        {
          results.AddRange(this.GetErrorsForTask(task, tParams));
        }
      }

      if (!results.Any())
      {
        results.Add(new ValidationResult(ValidatorState.Success, this.SuccessMessage, null));
      }

      return results;
    }

    protected abstract IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate);

    public Dictionary<string, string> Data { get; set; }

    protected virtual IEnumerable<string> GetParamNames()
    {
      if (this.Data.ContainsKey("ParamNames"))
      {
        return this.Data["ParamNames"].Split(',');
      }

      return new string[0];
    }
  }
}

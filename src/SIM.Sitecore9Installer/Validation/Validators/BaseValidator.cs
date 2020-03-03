using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public abstract class BaseValidator:IValidator
  {
    public BaseValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public virtual IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      List<ValidationResult> results = new List<ValidationResult>();
      foreach (Task task in tasks)
      {
        results.AddRange(this.GetErrorsForTask(task,task.LocalParams.Where(p=>this.GetParamNames().Contains(p.Name))));
      }

      if (!results.Any())
      {
        results.Add(new ValidationResult(ValidatorState.Success, string.Empty, null));
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

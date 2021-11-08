using System.Collections.Generic;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SolrServiceValidator : BaseValidator
  {
    public virtual SolrStateResolver SolrStateResolver => new SolrStateResolver();

    public override string SuccessMessage => "The 'Solr' service validation has been passed successfully.";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task,
      IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        if (SolrStateResolver.GetServiceState(param.Value) == SolrState.CurrentState.Stopped)
        {
          yield return new ValidationResult(ValidatorState.Error,
            $"The '{param.Value}' service required for the '{task.Name}' installation task is not running.", null);
        }
      }
    }
  }
}

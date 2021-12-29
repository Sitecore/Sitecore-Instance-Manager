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
        ServiceControllerWrapper serviceControllerWrapper = SolrStateResolver.GetService(param.Value);
        if (serviceControllerWrapper == null)
        {
          yield return new ValidationResult(ValidatorState.Success, 
            $"The '{param.Value}' service defined in the '{task.Name}' installation task does not exist, so the validation has been skipped.", null);

        }
        else if (SolrStateResolver.GetServiceState(serviceControllerWrapper) == SolrState.CurrentState.Stopped)
        {
          yield return new ValidationResult(ValidatorState.Error,
            $"The '{param.Value}' service required for the '{task.Name}' installation task is not running.", null);
        }
      }
    }
  }
}

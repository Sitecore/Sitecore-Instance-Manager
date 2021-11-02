using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SolrServiceValidator : BaseValidator
  {
    public override string SuccessMessage => "The 'Solr' service validation has been passed successfully.";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task,
      IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        ServiceControllerWrapper service = this.GetService(param.Value);
        if (service != null && service.Status != ServiceControllerStatus.Running)
        {
          yield return new ValidationResult(ValidatorState.Error,
            $"The '{param.Value}' service required for the '{task.Name}' installation task is not running.", null);
        }
      }
    }

    public virtual ServiceControllerWrapper GetService(string name)
    {
      ServiceController service= ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == name);
      if (service == null)
      {
        return null;
      }

      return new ServiceControllerWrapper(service);
    }
  }

  public class ServiceControllerWrapper
  {
    ServiceController _service;

    public ServiceControllerWrapper(ServiceController service)
    {
      this._service = service;
    }

    public virtual ServiceControllerStatus Status
    {
      get => this._service.Status;
    }

    public virtual string ServiceName
    {
      get => this._service.ServiceName;
    }
  }
}

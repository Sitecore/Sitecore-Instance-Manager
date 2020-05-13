using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class AppPoolSiteValidator : BaseValidator
  {
    protected virtual string AppPoolValidationMessage => "The '{0}' app pool already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    protected virtual string SiteValidationMessage => "The '{0}' site already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    public override string SuccessMessage => "IIS contains neither 'site' nor 'app pool' with the given name.";

    public List<ApplicationPool> AppPools { get; set; }

    public List<Site> Sites { get; set; }

    public AppPoolSiteValidator()
    {
      using (ServerManager manager = new ServerManager())
      {
        AppPools = manager.ApplicationPools.ToList();
        Sites = manager.Sites.ToList();
      }
    }

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        if (this.AppPoolExists(param.Value))
        {
          yield return new ValidationResult(ValidatorState.Error, string.Format(this.AppPoolValidationMessage, param.Value, param.Name, task.Name), null);
        }

        if (this.SiteExists(param.Value))
        {
          yield return new ValidationResult(ValidatorState.Error, string.Format(this.SiteValidationMessage, param.Value, param.Name, task.Name), null);
        }
      }
    }

    protected internal virtual bool AppPoolExists(string name)
    {
      return this.AppPools.Any(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    protected internal virtual bool SiteExists(string name)
    {
      return this.Sites.Any(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}

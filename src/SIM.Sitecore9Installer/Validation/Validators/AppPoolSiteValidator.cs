using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class AppPoolSiteValidator : IValidator
  {
    protected virtual string SiteName => "SiteName";

    protected virtual string AppPoolValidationMessage => "The '{0}' app pool already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    protected virtual string SiteValidationMessage => "The '{0}' site already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    public Dictionary<string, string> Data { get; set; }

    public List<ApplicationPool> AppPools { get; set; }

    public List<Site> Sites { get; set; }

    public AppPoolSiteValidator()
    {
      this.Data = new Dictionary<string, string>();
      using (ServerManager manager = new ServerManager())
      {
        AppPools = manager.ApplicationPools.ToList();
        Sites = manager.Sites.ToList();
      }
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      foreach (Task task in tasks.Where(t => t.LocalParams.Any(p => p.Name.Equals(this.SiteName, StringComparison.InvariantCultureIgnoreCase))))
      {
        string siteNameParam = task.LocalParams.Single(p => p.Name.Equals(this.SiteName, StringComparison.InvariantCultureIgnoreCase)).Value;

        if (this.AppPoolExists(siteNameParam))
        {
          yield return new ValidationResult(ValidatorState.Error, string.Format(this.AppPoolValidationMessage, siteNameParam, this.SiteName, task.Name), null);
        }

        if (this.SiteExists(siteNameParam))
        {
          yield return new ValidationResult(ValidatorState.Error, string.Format(this.SiteValidationMessage, siteNameParam, this.SiteName, task.Name), null);
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
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

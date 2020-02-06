using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Sitecore9Installer.Tasks;


namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostNameValidator:IValidator
  {
    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      List<ValidationResult> results = new List<ValidationResult>();
      foreach (Task task in tasks)
      {
        InstallParam dns = task.LocalParams.FirstOrDefault(p => p.Name == "DnsName");
        if (dns != null)
        {
          if (Uri.CheckHostName(dns.Value)!=UriHostNameType.Dns)
          {
            ValidationResult r = new ValidationResult(ValidatorState.Error,
              string.Format("Invalid host in '{0}' of '{1}'", dns.Name, task.Name), null);
            results.Add(r);
          }
        }
      }

      if (!results.Any())
      {
        results.Add(new ValidationResult(ValidatorState.Succsess, string.Empty, null));
      }

      return results;
    }
  }
}

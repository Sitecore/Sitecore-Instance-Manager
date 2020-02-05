using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Sitecore9Installer.Tasks;


namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostNameValidator:IValidator
  {
    public ValidationResult Evaluate(IEnumerable<Task> tasks)
    {
      StringBuilder errorMessage = new StringBuilder();
      foreach (Task task in tasks)
      {
        InstallParam dns = task.LocalParams.FirstOrDefault(p => p.Name == "DnsName");
        if (dns != null)
        {
          Uri uri;
          if (!Uri.TryCreate(dns.Value, UriKind.Absolute, out uri))
          {
            errorMessage.AppendFormat("Invalid host in '{0}' of '{1}'", dns.Name, task.Name);
            errorMessage.AppendLine();
          }
        }
      }

      if (errorMessage.Length > 0)
      {
        return new ValidationResult(ValidatorState.Error, errorMessage.ToString(), null);
      }

      return new ValidationResult(ValidatorState.Succsess, string.Empty, null);
    }
  }
}

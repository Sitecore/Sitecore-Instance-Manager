using System;
using System.Collections.Generic;
using System.Linq;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CmIdentityServerSiteNameValidator : IValidator
  {
    protected virtual string IdentityServerValidationResultMessage =>
      "The '{0}' parameter of the '{1}' task doesn't match the '{2}' parameter of the '{3}' task. To fix: set the correct value of the mentioned parameter in Advanced installation parameters.";

    public Dictionary<string, string> Data { get; set; }

    public CmIdentityServerSiteNameValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      string sitecoreXp1Cm = this.Data["SitecoreXp1Cm"];
      string sitecoreXm1Cm = this.Data["SitecoreXm1Cm"];
      string sitecoreXp0 = this.Data["SitecoreXp0"];
      string siteName = this.Data["SiteName"];
      string identityServer = this.Data["IdentityServer"];
      string allowedCorsOrigins = this.Data["AllowedCorsOrigins"];
      string passwordRecoveryUrl = this.Data["PasswordRecoveryUrl"];

      Task cmTask = tasks.FirstOrDefault(t => (t.Name.Equals(sitecoreXp1Cm, StringComparison.InvariantCultureIgnoreCase) || 
                                       t.Name.Equals(sitecoreXm1Cm, StringComparison.InvariantCultureIgnoreCase) ||
                                       t.Name.Equals(sitecoreXp0, StringComparison.InvariantCultureIgnoreCase)) && 
                                      t.LocalParams.Any(p => p.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase)));

      if (cmTask != null)
      {
        string cmSiteName = cmTask.LocalParams.Single(p => p.Name.Equals(siteName, StringComparison.InvariantCultureIgnoreCase)).Value;

        Task identityServerTask = tasks.FirstOrDefault(t => t.Name.Equals(identityServer, StringComparison.InvariantCultureIgnoreCase) &&
                                                    t.LocalParams.Any(p => p.Name.Equals(allowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase)) &&
                                                    t.LocalParams.Any(p => p.Name.Equals(passwordRecoveryUrl, StringComparison.InvariantCultureIgnoreCase)));

        if (identityServerTask != null)
        {
          Uri allowedCorsOriginsUri = new Uri(identityServerTask.LocalParams.Single(p => p.Name.Equals(allowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase)).Value);
          Uri passwordRecoveryUrlUri = new Uri(identityServerTask.LocalParams.Single(p => p.Name.Equals(passwordRecoveryUrl, StringComparison.InvariantCultureIgnoreCase)).Value);

          if (!cmSiteName.Equals(allowedCorsOriginsUri.Host, StringComparison.InvariantCultureIgnoreCase))
          {
            yield return new ValidationResult(ValidatorState.Warning,
              string.Format(IdentityServerValidationResultMessage, allowedCorsOrigins, identityServerTask.Name, siteName, cmTask.Name), null);
          }


          if (!cmSiteName.Equals(passwordRecoveryUrlUri.Host, StringComparison.InvariantCultureIgnoreCase))
          {
            yield return new ValidationResult(ValidatorState.Warning,
              string.Format(IdentityServerValidationResultMessage, passwordRecoveryUrl, identityServerTask.Name, siteName, cmTask.Name), null);
          }
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
    }
  }
}

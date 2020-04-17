using System;
using System.Collections.Generic;
using System.Linq;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CmIdentityServerSiteNameValidator : IValidator
  {
    private const string SitecoreXp1Cm = "sitecore-xp1-cm";

    private const string SitecoreXm1Cm = "Sitecore-xm1-cm";

    private const string SitecoreXm0 = "Sitecore-XP0";

    private const string SiteName = "SiteName";

    private const string IdentityServer = "IdentityServer";

    private const string AllowedCorsOrigins = "AllowedCorsOrigins";

    private const string PasswordRecoveryUrl = "PasswordRecoveryUrl";

    private const string IdentityServerValidationResultMessage =
      "The '{0}' parameter of the '{1}' task doesn't match the '{2}' parameter of the '{3}' task. To fix: set the correct value of the mentioned parameter in Advanced installation parameters.";


    public Dictionary<string, string> Data { get; set; }

    public CmIdentityServerSiteNameValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      Task cmTask = tasks.Single(t => (t.Name.Equals(SitecoreXp1Cm, StringComparison.InvariantCultureIgnoreCase) || 
                                       t.Name.Equals(SitecoreXm1Cm, StringComparison.InvariantCultureIgnoreCase) ||
                                       t.Name.Equals(SitecoreXm0, StringComparison.InvariantCultureIgnoreCase)) && 
                                      t.LocalParams.Any(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)));

      if (cmTask != null)
      {
        string cmSiteName = cmTask.LocalParams.Single(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)).Value;

        Task identityServerTask = tasks.Single(t => t.Name.Equals(IdentityServer, StringComparison.InvariantCultureIgnoreCase) &&
                                                    t.LocalParams.Any(p => p.Name.Equals(AllowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase)) &&
                                                    t.LocalParams.Any(p => p.Name.Equals(PasswordRecoveryUrl, StringComparison.InvariantCultureIgnoreCase)));

        if (identityServerTask != null)
        {
          Uri allowedCorsOriginsUri = new Uri(identityServerTask.LocalParams.Single(p => p.Name.Equals(AllowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase)).Value);
          Uri passwordRecoveryUrlUri = new Uri(identityServerTask.LocalParams.Single(p => p.Name.Equals(PasswordRecoveryUrl, StringComparison.InvariantCultureIgnoreCase)).Value);

          if (cmSiteName != allowedCorsOriginsUri.Host)
          {
            yield return new ValidationResult(ValidatorState.Warning,
              string.Format(IdentityServerValidationResultMessage, AllowedCorsOrigins, identityServerTask.Name, SiteName, cmTask.Name), null);
          }


          if (cmSiteName != passwordRecoveryUrlUri.Host)
          {
            yield return new ValidationResult(ValidatorState.Warning,
              string.Format(IdentityServerValidationResultMessage, PasswordRecoveryUrl, identityServerTask.Name, SiteName, cmTask.Name), null);
          }
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CmDdsPatchSiteNameValidator : IValidator
  {
    protected virtual string SitecoreXp1Cm => "sitecore-xp1-cm";

    protected virtual string SitecoreXp1CmDdsPatch => "sitecore-XP1-cm-dds-patch";

    protected virtual string SiteName => "SiteName";

    protected virtual string DdsPatchValidationResultMessage =>
      "Value of the '{0}' parameter differs in the '{1}' and '{2}' tasks. To fix: set the same value for each mentioned task in Advanced installation parameters.";

    public Dictionary<string, string> Data { get; set; }

    public CmDdsPatchSiteNameValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      Task cmTask = tasks.FirstOrDefault(t => t.Name.Equals(SitecoreXp1Cm, StringComparison.InvariantCultureIgnoreCase) && 
                                      t.LocalParams.Any(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)));

      if (cmTask != null)
      {
        Task ddsPatchTask = tasks.FirstOrDefault(t => t.Name.Equals(SitecoreXp1CmDdsPatch, StringComparison.InvariantCultureIgnoreCase) &&
                                              t.LocalParams.Any(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)));

        if (ddsPatchTask != null)
        {
          string cmSiteName = cmTask.LocalParams.Single(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)).Value;
          string ddsPatchSiteName = ddsPatchTask.LocalParams.Single(p => p.Name.Equals(SiteName, StringComparison.InvariantCultureIgnoreCase)).Value;

          if (!cmSiteName.Equals(ddsPatchSiteName, StringComparison.InvariantCultureIgnoreCase))
          {
            yield return new ValidationResult(ValidatorState.Error,
              string.Format(DdsPatchValidationResultMessage, SiteName, cmTask.Name, ddsPatchTask.Name), null);
          }
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
    }
  }
}

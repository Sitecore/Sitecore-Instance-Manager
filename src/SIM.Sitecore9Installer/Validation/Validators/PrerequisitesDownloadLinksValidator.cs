﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class PrerequisitesDownloadLinksValidator : BaseValidator
  {
    public override string SuccessMessage => "Prerequisites download links are valid.";

    protected virtual string TaskName => "Prerequisites";

    // parameter to validate the following known issue: https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Outdated-Download-Link-to-Microsoft-Web-Platform-Installer
    public virtual string WebPlatformDownload => "WebPlatformDownload";

    public virtual string OutdatedDownloadLinksKnownIssueLink => "https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Outdated-download-links-in-Prerequisites.json";

    public virtual string InternetAccessKnownIssueLink => "https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Prerequisites-require-Internet-access-to-be-installed";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      string paramNamePostfix = string.Empty;
      if (this.Data.ContainsKey("ParamNamePostfix"))
      {
        paramNamePostfix = this.Data["ParamNamePostfix"];
      }

      List<string> paramValuePrefixes = new List<string>();
      if (this.Data.ContainsKey("ParamValuePrefixes"))
      {
        paramValuePrefixes = this.Data["ParamValuePrefixes"].Split('|').ToList();
      }

      if (!string.IsNullOrEmpty(paramNamePostfix) && paramValuePrefixes.Count > 0)
      {
        if (task.Name.Equals(TaskName, StringComparison.InvariantCultureIgnoreCase))
        {
          foreach (InstallParam installParam in paramsToValidate)
          {
            if (paramValuePrefixes.Any(paramValuePrefix => installParam.Value.StartsWith(paramValuePrefix, StringComparison.InvariantCultureIgnoreCase)))
            {
              if (!this.IsDownloadLinkValid(installParam.Value))
              {
                if (installParam.Name == this.WebPlatformDownload)
                {
                  yield return new ValidationResult(ValidatorState.Warning,
                    $"{TaskName}: the '{installParam.Name}' parameter contains the following link that is not accessible:\n\n{installParam.Value}\n\nThis behavior looks to be related to the following known issue:\n\n{OutdatedDownloadLinksKnownIssueLink}\n\nPlease try to apply the solution mentioned there.",
                    null);
                }
                else
                {
                  yield return new ValidationResult(ValidatorState.Warning,
                    $"{TaskName}: the '{installParam.Name}' parameter contains the following link that is not accessible:\n\n{installParam.Value}\n\nPlease check the link accessibility in a browser and solution mentioned in the following known issue:\n\n{InternetAccessKnownIssueLink}",
                    null);
                }
              }
            }
            else if (installParam.Name.EndsWith(paramNamePostfix, StringComparison.InvariantCultureIgnoreCase))
            {
              yield return new ValidationResult(ValidatorState.Warning,
                $"{TaskName}: the '{installParam.Name}' parameter contains the following invalid value:\n\n{installParam.Value}\n\nIt should contain download link that starts with '{string.Join("' or '", paramValuePrefixes)}'.",
                null);
            }
          }
        }
      }
    }

    private bool IsDownloadLinkValid(string link)
    {
      using (HttpClient authClient = new HttpClient())
      {
        try
        {
          var response = authClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, new Uri(link))).Result;
          if (response.IsSuccessStatusCode)
          {
            return true;
          }
        }
        catch
        {
          return false;
        }
      }

      return false;
    }
  }
}
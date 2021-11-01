using System.Collections.Generic;
using System.Net;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostAvaiableValidator : BaseValidator
  {
    public override string SuccessMessage => null;

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Tasks.Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam p in paramsToValidate)
      {
        WebException error = null;
        HttpWebResponse resp = null;
        try
        {
          resp = this.GetResponse(p.Value);
        }
        catch(WebException ex)
        {
          error = ex;
        }

        if (error != null)
        {
          yield return new ValidationResult(ValidatorState.Error, $"Unable to connect to the following host:\n{p.Value}", error);
          yield break;
        }

        if (resp.StatusCode != HttpStatusCode.OK)
        {
          yield return new ValidationResult(ValidatorState.Error, $"The following host did not return status code 200:\n{p.Value}",null);
        }
        else
        {
          yield return new ValidationResult(ValidatorState.Success, $"The following host returned status code 200:\n{p.Value}", null);
        }
      }
    }

    protected internal virtual HttpWebResponse GetResponse(string uri)
    {
      HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
      return (HttpWebResponse)myReq.GetResponse();
    }
  }
}

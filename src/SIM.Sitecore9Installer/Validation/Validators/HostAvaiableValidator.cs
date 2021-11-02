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
          yield return new ValidationResult(ValidatorState.Error, $"Unable to connect to the '{p.Value}' host defined in the '{p.Name}' parameter of the '{task.Name}' installation task.", error);
          yield break;
        }

        if (resp.StatusCode != HttpStatusCode.OK)
        {
          yield return new ValidationResult(ValidatorState.Error, $"The '{p.Value}' host defined in the '{p.Name}' parameter of the '{task.Name}' installation task did not return status code 200.", null);
        }
        else
        {
          yield return new ValidationResult(ValidatorState.Success, $"The '{p.Value}' host defined in the '{p.Name}' parameter of the '{task.Name}' installation task returned status code 200.", null);
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

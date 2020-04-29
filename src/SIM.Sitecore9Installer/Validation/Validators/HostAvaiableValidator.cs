using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class HostAvaiableValidator : BaseValidator
  {
    protected override IEnumerable<ValidationResult> GetErrorsForTask(Tasks.Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam p in paramsToValidate)
      {
        HttpWebResponse resp = this.GetResponse(p.Value);
        if (resp.StatusCode != HttpStatusCode.OK)
        {
          yield return new ValidationResult(ValidatorState.Error, $"Host {p.Value} did not return 200.",null);
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

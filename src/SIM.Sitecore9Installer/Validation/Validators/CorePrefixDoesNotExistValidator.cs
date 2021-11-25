using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CorePrefixDoesNotExistValidator : IValidator
  {
    public CorePrefixDoesNotExistValidator()
    {
      this.Data = new Dictionary<string, string>();
    }

    public virtual string SuccessMessage => "Solr core prefix is unique.";

    public string CorePrefix { get => this.Data["Prefix"]; }
    public string SolrUrl { get => this.Data["Solr"]; }
    public Dictionary<string, string> Data { get; set; }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Tasks.Task> tasks)
    {
      bool errors = false;
      List<Tuple<string, string>> checkList = new List<Tuple<string, string>>();
      foreach (Task task in tasks)
      {
        InstallParam prefix = task.LocalParams.FirstOrDefault(p => p.Name == this.CorePrefix);
        InstallParam url = task.LocalParams.FirstOrDefault(p => p.Name == this.SolrUrl);
        if (prefix != null && url != null && !checkList.Any(t => t.Item1.Equals(url.Value, StringComparison.InvariantCultureIgnoreCase) && t.Item2.Equals(prefix.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
          checkList.Add(new Tuple<string, string>(url.Value, prefix.Value + "_"));
        }
      }

      foreach (Tuple<string, string> item in checkList)
      {
        WebException errorGetcores = null;
        IEnumerable<string> coreNames = null;
        try
        {
          coreNames=this.GetCores(item.Item1);
        }
        catch(WebException ex)
        {
          errorGetcores = ex;
        }

        if (errorGetcores != null)
        {
          yield return new ValidationResult(ValidatorState.Error, $"Unable to get cores from Solr '{item.Item1}'.", errorGetcores);
          continue;
        }

        if (coreNames.Any(cn => cn.StartsWith(item.Item2, StringComparison.InvariantCultureIgnoreCase)))
        {
          errors = true;
          yield return new ValidationResult(ValidatorState.Error, $"Core with the '{item.Item2}' prefix already exists in Solr '{item.Item1}'.", null);
        }
      }

      if (!errors)
      {
        yield return new ValidationResult(ValidatorState.Success, this.SuccessMessage, null);
      }
    }


    protected internal virtual IEnumerable<string> GetCores(string uri)
    {
      HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(Path.Combine(uri, "admin/cores?action=STATUS&wt=json"));
      HttpWebResponse resp = (HttpWebResponse)myReq.GetResponse();
      using (StreamReader reader = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(resp.CharacterSet)))
      {
        string responseText = reader.ReadToEnd();
        JObject doc = JObject.Parse(responseText);
        return doc["status"]?.Children().Select(c => c as JProperty).Select(jp => jp.Name);
      }

    }
  }
}

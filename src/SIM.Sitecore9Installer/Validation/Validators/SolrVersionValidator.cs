using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SolrVersionValidator : IValidator
  {
    public virtual string SuccessMessage => "Sitecore XP and 'Solr' versions are compatible.";
    public string SolrUrl { get => this.Data["Solr"]; }

    public Dictionary<string, string> Data { get; set; }

    public SolrVersionValidator()
    {
      Data = new Dictionary<string, string>();
    }

    private class SolrVersionValidatorErrors
    {
      internal List<string> oldSolrTaskNames = new List<string>();
      internal List<string> unresolvedSolrTaskNames = new List<string>();
      internal List<string> untestedSolrTaskNames = new List<string>();
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Tasks.Task> tasks)
    {
      string[] compatibleVersions = Data["Versions"].Split(',');
      Array.Sort(compatibleVersions);

      if (compatibleVersions.Length == 0)
      {
        yield return new ValidationResult(ValidatorState.Success, this.SuccessMessage, null);
      }

      SolrVersionValidatorErrors errors = GetErrors(tasks, compatibleVersions);

      if (!errors.oldSolrTaskNames.Any() 
        && !errors.unresolvedSolrTaskNames.Any() 
        && !errors.untestedSolrTaskNames.Any())
      {
        yield return new ValidationResult(ValidatorState.Success, this.SuccessMessage, null);
      }

      if (errors.oldSolrTaskNames.Any())
      {
        yield return new ValidationResult(ValidatorState.Error,
          $"'Solr' version is older than expected. Please consider a newer version. " +
          $"Recommended 'Solr' versions: {Data["Versions"]}. " +
          $"Installation tasks related to the problematic Solr versions: {string.Join(", ", errors.oldSolrTaskNames)}",
          null);
      }

      if (errors.unresolvedSolrTaskNames.Any())
      {
        yield return new ValidationResult(ValidatorState.Error,
          $"Unable to resolve 'Solr' versions for the following tasks: {string.Join(", ", errors.unresolvedSolrTaskNames)}",
          null);
      }

      if (errors.untestedSolrTaskNames.Any())
      {
        yield return new ValidationResult(ValidatorState.Warning,
          $"Sitecore XP and 'Solr' versions have not been tested together. They might be incompatible. " +
          $"Recommended 'Solr' versions: {Data["Versions"]}. " +
          $"Installation tasks related to the problematic Solr versions: {string.Join(", ", errors.untestedSolrTaskNames)}", 
          null);
      }   
    }

    private SolrVersionValidatorErrors GetErrors(IEnumerable<Tasks.Task> tasks, string[] compatibleVersions)
    {
      SolrVersionValidatorErrors errors = new SolrVersionValidatorErrors();
      foreach (Task task in tasks.Where(t => t.LocalParams.Any(p => p.Name == SolrUrl)))
      {
        string solrUrl = task.LocalParams.Single(p => p.Name == SolrUrl).Value;
        string solrVersion = string.Empty;

        try
        {
          solrVersion = GetSolrVersion(solrUrl);
        }
        catch
        {
          errors.unresolvedSolrTaskNames.Add(task.Name);
        }

        if (!string.IsNullOrEmpty(solrVersion) 
          && !compatibleVersions.Any(v => Regex.Match(solrVersion, v).Success))
        {
          if (string.Compare(solrVersion, compatibleVersions[0], StringComparison.OrdinalIgnoreCase) < 0)
          {
            errors.oldSolrTaskNames.Add($"{task.Name}({solrVersion})");
          }
          else
          {
            errors.untestedSolrTaskNames.Add($"{task.Name}({solrVersion})");
          }
        }
      }

      return errors;
    }

    private string GetSolrVersion(string solrUrl)
    {
      HttpClient client = new HttpClient();

      using (Stream stream = client.GetStreamAsync($"{solrUrl}/admin/info/system").Result)
      using (StreamReader streamReader = new StreamReader(stream))
      using (JsonReader reader = new JsonTextReader(streamReader))
      {
        while (reader.Read())
        {
          if (string.Equals(reader.Path, "lucene.solr-spec-version",
            StringComparison.OrdinalIgnoreCase)
            && !string.Equals((string)reader.Value, "solr-spec-version",
            StringComparison.OrdinalIgnoreCase))
          {
            return (string)reader.Value;
          }
        }
      }

      return string.Empty;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SIM.Products;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SolrVersionValidator : IValidator
  {
    public virtual SolrStateResolver SolrStateResolver => new SolrStateResolver();

    public virtual string SuccessMessage => "Sitecore and Solr versions are compatible.";

    public Dictionary<string, string> Data { get; set; }

    public string SolrUrl { get => this.Data["SolrUrl"]; }

    public string FilesRoot { get => this.Data["FilesRoot"]; }

    public SolrVersionValidator()
    {
      Data = new Dictionary<string, string>();
    }

    private class SolrVersionValidatorErrors
    {
      internal List<string> unresolvedSolrTasks = new List<string>();
      internal List<string> incompatibleSolrTasks = new List<string>();
    }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      Task globalTask = tasks.FirstOrDefault(t => t.GlobalParams.Any(p => p.Name.Equals(FilesRoot, StringComparison.InvariantCultureIgnoreCase)));

      if (globalTask == null)
      {
        yield return new ValidationResult(ValidatorState.Warning,
          $"Cannot find installation task that contains the '{FilesRoot}' parameter to get Sitecore version and to validate Solr version.", null);
        yield break;
      }

      Regex productRegex = Product.ProductRegex;
      Match match = productRegex.Match(globalTask.GlobalParams[FilesRoot].Value);
      string sitecoreVersion = match.Groups[2].Value;

      if (string.IsNullOrEmpty(sitecoreVersion))
      {
        yield return new ValidationResult(ValidatorState.Warning,
          $"Cannot resolve Sitecore version using the '{FilesRoot}' parameter of the '{globalTask.Name}' installation task to validate Solr version.", null);
        yield break;
      }

      Dictionary<string, string> solrVersionMaps = Configuration.Configuration.Instance.SolrMap;

      if (solrVersionMaps == null || solrVersionMaps.Count < 1)
      {
        yield return new ValidationResult(ValidatorState.Warning, 
          "Cannot resolve Sitecore and Solr versions mappings from the 'GlobalSettings.json' file to validate Solr version.", null);
        yield break;
      }

      KeyValuePair<string, string> solrVersionsMap = new KeyValuePair<string, string>();

      foreach (string value in solrVersionMaps.Values)
      {
        string[] splittedValues = value.Split(',');
        foreach (string splittedValue in splittedValues)
        {
          if (Regex.Match(sitecoreVersion, splittedValue).Success)
          {
            solrVersionsMap = solrVersionMaps.FirstOrDefault(keyValuePair => keyValuePair.Value.Contains(splittedValue));
            break;
          }
        }
      }

      if (string.IsNullOrEmpty(solrVersionsMap.Key))
      {
        yield return new ValidationResult(ValidatorState.Warning, 
          $"Cannot resolve Sitecore version {sitecoreVersion} from 'SolrVersionsMap' in the 'GlobalSettings.json' file to validate Solr version.", null);
        yield break;
      }

      SolrVersionValidatorErrors errors = this.GetErrors(tasks, solrVersionsMap);

      if (!errors.unresolvedSolrTasks.Any() && !errors.incompatibleSolrTasks.Any())
      {
        yield return new ValidationResult(ValidatorState.Success, this.SuccessMessage, null);
        yield break;
      }

      if (errors.unresolvedSolrTasks.Any())
      {
        yield return new ValidationResult(ValidatorState.Error,
          $"Unable to resolve Solr versions for the following tasks:\n\n{string.Join("\n", errors.unresolvedSolrTasks)}", null);
      }

      if (errors.incompatibleSolrTasks.Any())
      {
        yield return new ValidationResult(ValidatorState.Warning,
          $"Sitecore version {sitecoreVersion} and Solr versions defined in the following installation tasks have not been tested together:\n\n{string.Join("\n", errors.incompatibleSolrTasks)}\n\n" +
          $"The recommended compatible Solr version is {solrVersionsMap.Key}.", null);
      }
    }

    private SolrVersionValidatorErrors GetErrors(IEnumerable<Task> tasks, KeyValuePair<string, string> solrVersionsMap)
    {
      SolrVersionValidatorErrors errors = new SolrVersionValidatorErrors();

      foreach (Task task in tasks.Where(t => t.LocalParams.Any(p => p.Name == SolrUrl)))
      {
        string solrUrl = task.LocalParams.Single(p => p.Name == SolrUrl).Value;
        string solrVersion = string.Empty;

        try
        {
          solrVersion = SolrStateResolver.GetVersion(solrUrl);
        }
        catch
        {
          errors.unresolvedSolrTasks.Add($"{task.Name} ({solrUrl})");
        }

        if (!string.IsNullOrEmpty(solrVersion) && !Regex.Match(solrVersion, solrVersionsMap.Key).Success)
        {
          errors.incompatibleSolrTasks.Add($"{task.Name} ({solrUrl} - {solrVersion})");
        }
      }

      return errors;
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SitecoreInstaller.Validation.Abstractions;
using SIM.Sitecore9Installer.Validation.Factory;
using SIM.Sitecore9Installer.Validation.Solr;

namespace SitecoreInstaller.Validation.Solr
{
  public class SolrCorePrefixValidator : IInstallationValidator
  {
    private Dictionary<string, string> installParams;

    public SolrCorePrefixValidator()
    {
      Name = "SolrCorePrefixValidator";
    }
    private string solrUrl = "";

    public SolrCorePrefixValidator(Dictionary<string, string> installParams):this()
    {
      this.installParams = installParams;
    }

    public bool CoreExists(string name)
    {
    //  var headerParser = new HeaderResponseParser<string>();
    //  var statusParser = new SolrStatusResponseParser();
     // CoreResult result = null;
      bool result = false;
      try
      {
        using (var solrCoreAdmin = new SolrCoreAdminWrapper())
        {
          result = solrCoreAdmin.Status(name);
        }
      }
      catch (HttpRequestException solrEx)
      {
        this.Details += " Solr connection failed ";
      }
      catch (System.Exception ex)
      {
        this.Details += " Unspecified error ";
      }
        return result;
    }

    public List<string> FindCoresByPrefix(string prefix)
    {
      List<string> result = new List<string>();

      using (var solrCoreAdmin = new SolrCoreAdminWrapper())
      {
        var allCores = solrCoreAdmin.GetCoreNamesFromResponse();
        foreach (var coreResult in allCores)
        {
          if (coreResult.StartsWith(prefix))
            result.Add(coreResult);
        }
      }
      return result;
    }

    public IEnumerable<string> GetAllCoreNames(string prefix)
    {
      //TODO: json parser for all core names
      return null;
    }
    //todo: move to remover
    public int RemoveAllCores(string prefix)
    {
      return -1;
    }
    //todo: move to remover
    public bool RemoveCore(string name)
    {
      int response = 1;
      using (var solrCoreAdmin = new SolrCoreAdminWrapper())
      { 
        response = solrCoreAdmin.Unload(name, "DeleteInstance");
      }
      return response == 0;
    }

    public string Name { get; set; }

    private bool IsValidated = false;

    public ValidationResult Result
    {
      get;
      private set;
    }


    public ValidationResult Validate(Dictionary<string, string> installParams)
    {
      this.Result = ValidationResult.None;
      this.Details = "Validator under reconstruction";
      return this.Result;

      this.solrUrl = installParams["SolrUrl"];
      if (this.installParams != null && this.installParams.Count > 0)
        return this.Validate();
      var solrCorePrefix = InstallParamsHelper.GetActualValue("SolrCorePrefix", installParams);

      if (solrCorePrefix != null)
      {
        List<string> coreNames = this.FindCoresByPrefix(solrCorePrefix);
        if (coreNames.Count == 0)
        {
          this.Details = "No cores with prefix " + solrCorePrefix + " found";
          this.Result = ValidationResult.Ok;
          return ValidationResult.Ok;
        }
        else
        {
          foreach (var coreName in coreNames)
          {
            this.Details += coreName + " ";
          }

          this.Details += " already exist";
          this.Result = ValidationResult.Error;
          return ValidationResult.Error;
        }
      }
      return ValidationResult.None;
    }

    public ValidationResult Validate()
    {
      if (this.installParams["SolrCorePrefix"] != null)
      {
        List<string> coreNames = this.FindCoresByPrefix(this.installParams["SolrCorePrefix"]);
        if (coreNames.Count == 0)
        {
          this.Details = "No cores with prefix " + this.installParams["SolrCorePrefix"] + " found";
          this.Result = ValidationResult.Ok;
          return ValidationResult.Ok;
        }
        else
        {
          foreach (var coreName in coreNames)
          {
            this.Details += coreName + " ";
          }

          this.Details += " already exist";
          this.Result = ValidationResult.Error;
          return ValidationResult.Error;
        }
      }
      return ValidationResult.Warning;
    }

    public string Details { get; set; }

    public ValidationResult Validate(string siteName)
    {
      return ValidationResult.Warning;
    }
  }
}

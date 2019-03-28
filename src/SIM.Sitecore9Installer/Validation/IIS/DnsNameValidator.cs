using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SitecoreInstaller.Validation.Abstractions;

namespace SitecoreInstaller.Validation.IIS
{
  public class SiteNameValidator : IInstallationValidator
  {
    private ServerManager serverManager;

    public SiteNameValidator()
    {
      serverManager = new ServerManager();
      Name = "SiteNameIISValidator";
    }

    public bool WebSiteExists(string name)
    {
      return serverManager.Sites.Any(x => x.Name == name);
    }

    public bool AppPoolExists(string name)
    {
      return serverManager.ApplicationPools.Any(x => x.Name == name);
    }

    public string Name { get; set; }

    public ValidationResult Result { get; set; }
    public ValidationResult Validate(Dictionary<string, string> installParams)
    {
      bool webSite = WebSiteExists(installParams["SiteName"]);
      bool appPool = AppPoolExists(installParams["SiteName"]);
      if (webSite && appPool)
      {
        this.Result = ValidationResult.Error;
        this.Details = "Website and application pool with the same name exist";
        return ValidationResult.Error;
      }

      if (webSite || appPool)
      {
        this.Result = ValidationResult.Warning;
        this.Details = "Website or application pool with the same name exist";
      }
      else
      {
        this.Result = ValidationResult.Ok;
        this.Details = "No site or app pool with such name found locally";
      }

      return this.Result;
    }

    public ValidationResult Validate()
    {
      throw new System.NotImplementedException();
    }

    public string Details { get ; set; }

    public ValidationResult Validate(string siteName)
    {
      return ValidationResult.Warning;
    }
  }
}

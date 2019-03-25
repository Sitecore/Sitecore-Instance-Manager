using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SitecoreInstaller.Validation.Abstractions;

namespace SitecoreInstaller.Validation.IIS
{
  public class DnsNameValidator : IInstallationValidator
  {
    private ServerManager serverManager;

    public DnsNameValidator()
    {
      serverManager = new ServerManager();
      Name = "DnsNameValidator";
    }

    public static DnsNameValidator Instance => new DnsNameValidator();

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
      bool webSite = WebSiteExists(installParams["DnsName"]);
      bool appPool = AppPoolExists(installParams["DnsName"]);
      if (webSite && appPool)
      {
        return ValidationResult.Error;
      }

      if (webSite || appPool)
      {
        return ValidationResult.Warning;
      }
      else
      {
        return ValidationResult.Ok;
      }
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

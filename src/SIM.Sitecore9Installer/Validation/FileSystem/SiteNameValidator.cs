using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SitecoreInstaller.Validation.Abstractions;

namespace SitecoreInstaller.Validation.FileSystem
{
  public class SiteNameValidator:IInstallationValidator
  {
    public string Name
    {
      get { return "SiteName";}
      set { Name = value; }
    }
    public ValidationResult Result { get; private set; }

    public async NotifyTaskCompletion<ValidationResult> ResultAsync
    {
      get; private set;
    }


    public ValidationResult Validate(string siteName)
    {
      var result = FolderChecks.ValidatePath(siteName);
      if (result)
      {
        this.Details = "Path for the site already exists:" + FolderChecks.GetSitePath(siteName);
        Result = ValidationResult.Warning;
        return ValidationResult.Warning;
      }
      else
      {
        this.Details = "Path is not present";
        Result = ValidationResult.Ok;
        return ValidationResult.Ok;
      }
    }

    public ValidationResult Validate(Dictionary<string, string> installParams)
    {
      return Validate(installParams["SiteName"]);
    }

    public ValidationResult Validate()
    {
      throw new NotImplementedException();
    }

    public string Details { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitecoreInstaller.Validation.Abstractions;

namespace SIM.Sitecore9Installer.Validation.Factory
{
  public class ValidatorViewModel
  {
    private IInstallationValidator validator;

    public ValidatorViewModel(IInstallationValidator validator, Dictionary<string, string> installParams)
    {
      this.validator = validator;
      validator.Validate(installParams);
    }

    public string Name { get
      {
        return validator.Name;
      
    } }

    public ValidationResult Result
    {
      get { return validator.Result; }
    }

    public string Details
    {
      get { return validator.Details; }
    }


  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class LicenseFileValidator : IValidator
  {
    public LicenseFileValidator()
    {
      this.Data = new Dictionary<string, string>();
    }
    public Dictionary<string, string> Data { get; set; }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Tasks.Task> tasks)
    {
      bool hasErrors = false;
      IEnumerable<InstallParam> licenseFilePaths = tasks.Select(t => t.LocalParams.FirstOrDefault(l => l.Name == this.Data["LicenseFileVariable"]))
        .Where(p => p != null);
      IEnumerable<string> uniquelicenseFilePaths = licenseFilePaths.Select(x => x.Value).Distinct();

      foreach (string path in uniquelicenseFilePaths)
      {
        if (!FileExists(path))
        {
          yield return new ValidationResult(ValidatorState.Error, $"Unable to locate {path} license file.", null);
          hasErrors = true;
        }
      }

      if (!hasErrors)
      {
        yield return new ValidationResult(ValidatorState.Success, null, null);
      }
    }

    protected internal virtual bool FileExists(string path) => File.Exists(path);

  }
}

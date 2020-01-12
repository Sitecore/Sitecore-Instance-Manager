using System.Collections.Generic;

namespace SitecoreInstaller.Validation.Abstractions
{
  public interface IInstallationValidator
  {

    string Name { get; set; }

    ValidationResult Result { get; }

    ValidationResult Validate(string siteName);

    //TODO: remove later
    ValidationResult Validate(Dictionary<string, string> installParams);

    ValidationResult Validate();

    string Details { get; set; }
  }
  public enum ValidationResult
  {
    None,
    Error,
    Warning,
    Ok
  }
}

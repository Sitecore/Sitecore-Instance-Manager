using System.Collections.Generic;
using System.Management.Automation;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class NugetToolPresenceValidator : BaseValidator
  {
    protected virtual string TaskName => "sitecore-XP1-cm-dds-patch";

    protected virtual string CommandToEvaluate => "nuget.exe";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      if (task.Name.Equals(TaskName, System.StringComparison.InvariantCultureIgnoreCase))
      {
        IEnumerable<ValidationResult> result = GetValidationResults();

        return result;
      }

      return new ValidationResult[0];
    }

    protected virtual string GetScript(string command)
    {
      // Try to run 'nuget.exe' tool without any parameters.
      // In case the tool is absent the CommandNotFoundException is thrown.

      string script = command;

      return script;
    }

    protected virtual IEnumerable<ValidationResult> GetValidationResults()
    {
      string script = GetScript(CommandToEvaluate);

      string psError = GetScriptError(script);

      if (psError.Equals("CommandNotFoundException", System.StringComparison.InvariantCultureIgnoreCase))
      {
        ValidationResult result = new ValidationResult(
          state: ValidatorState.Error,
          message: $"{TaskName}: nuget.exe is not a recognized as an internal or external command. To fix: download the NuGet.exe from https://www.nuget.org/downloads and set the local path of NuGet.exe into your path environment variable.",
          error: null);

        yield return result;
      }
      else if (!string.IsNullOrEmpty(psError))
      {
        ValidationResult result = new ValidationResult(
          state: ValidatorState.Error,
          message: $"{TaskName}: The '{psError}' error occurred when 'nuget.exe' was invoked from PowerShell.",
          error: null);

        yield return result;
      }
    }

    protected virtual string GetScriptError(string script)
    {
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(script);
        PowerShellInstance.Invoke();

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          return PowerShellInstance.Streams.Error[0]?.FullyQualifiedErrorId ?? string.Empty;
        }
        else
        {
          return string.Empty;
        }
      }
    }
  }
}
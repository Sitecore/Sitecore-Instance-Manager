using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class PrerequisitesDownloadLinksValidatorTests
  {
    private const string KnownIssueMessage = "{0}: the '{1}' parameter contains the following invalid link that is not accessible:\n\n{2}\n\nThis behavior looks to be related to the following known issue:\n\nhttps://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Outdated-Download-Link-to-Microsoft-Web-Platform-Installer\n\nPlease try to apply the solution mentioned there.";

    private const string InvalidLinkMessage = "{0}: the '{1}' parameter contains the following invalid link that is not accessible:\n\n{2}\n\nThis behavior may occur due to similar symptoms described in the following known issue:\n\nhttps://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Outdated-Download-Link-to-Microsoft-Web-Platform-Installer";

    private const string InvalidValueMessage = "{0}: the '{1}' parameter contains the following invalid value:\n\n{2}\n\nIt should contain download link that starts with '{3}'.";

    [Theory]
    [InlineData("Prerequisites", "WebPlatformDownload", "https://download.microsoft.com/download/C/F/F/CFF3A0B8-99D4-41A2-AE1A-496C08BEB904/WebPlatformInstaller_amd64_en-US.msi", 1, KnownIssueMessage)]
    [InlineData("Global", "WebPlatformDownload", "https://download.microsoft.com/download/C/F/F/CFF3A0B8-99D4-41A2-AE1A-496C08BEB904/WebPlatformInstaller_amd64_en-US.msi", 0, "")]
    [InlineData("Prerequisites", "SQLODBCDriversx64", "https://download.microsoft.com/download/D/5/E/D5EEF288-A277-45C8-855B-8E2CB7E25B96/x64/msodbcsql.msi", 0, "")]
    [InlineData("Prerequisites", "SQLODBCDriversx64", "https://download.microsoft.com/download/test", 1, InvalidLinkMessage)]
    [InlineData("Prerequisites", "SQLODBCDriversx64", "test", 0, "")]
    [InlineData("Prerequisites", "DotNetHostingDownload", "https://download.microsoft.com/download/6/E/B/6EBD972D-2E2F-41EB-9668-F73F5FDDC09C/dotnet-hosting-2.1.3-win.exe", 0, "")]
    [InlineData("Prerequisites", "DotNetHostingDownload", "test", 1, InvalidValueMessage)]
    public void EvaluateTests(string taskName, string paramName, string paramValue, int warningsCount, string message)
    {
      // Arrange
      var fixture = new Fixture();
      GlobalParameters globals = new GlobalParameters();
      Task prerequisitesTask = Substitute.For<Task>(taskName, fixture.Create<int>(), null, null, new Dictionary<string, string>());
      InstallParam downloadLinkParam = new InstallParam(paramName, paramValue, false, InstallParamType.String);
      List<InstallParam> paramList = new List<InstallParam>
      {
        downloadLinkParam
      };
      LocalParameters locals = new LocalParameters(paramList, globals);
      prerequisitesTask.LocalParams.Returns(locals);

      PrerequisitesDownloadLinksValidator validator = Substitute.ForPartsOf<PrerequisitesDownloadLinksValidator>();
      validator.Data["ParamNamePostfix"] = "Download";
      validator.Data["ParamValuePrefixes"] = "http://|https://";
      List<string> paramValuePrefixes = validator.Data["ParamValuePrefixes"].Split('|').ToList();

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] {prerequisitesTask});
      IEnumerable<ValidationResult> warnings = result.Where(r => r.State == ValidatorState.Warning);

      // Assert
      Assert.Equal(warnings.Count(), warningsCount);
      this.ValidateMessage(warnings, message, taskName, paramName, paramValue, string.Join("' or '", paramValuePrefixes));
    }

    private void ValidateMessage(IEnumerable<ValidationResult> warnings, string message, string taskName, string paramName, string paramValue, string paramValuePrefixes)
    {
      if (!string.IsNullOrEmpty(message))
      {
        message = string.Format(message, taskName, paramName, paramValue, paramValuePrefixes);
        Assert.Contains(warnings, warning => warning.Message.Equals(message));
      }
    }
  }
}
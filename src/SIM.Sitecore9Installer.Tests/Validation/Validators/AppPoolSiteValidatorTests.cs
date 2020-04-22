using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NSubstitute;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class AppPoolSiteValidatorTests
  {
    private const string TaskName = "Sitecore-XP0";

    private const string SiteName = "SiteName";

    private const string AppPoolValidationMessage = "The '{0}' app pool already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    private const string SiteValidationMessage = "The '{0}' site already exists in IIS. To fix: change the '{1}' field's value of the '{2}' task in Advanced installation parameters.";

    [Theory]
    [InlineData("sc.local", true, true, 2, AppPoolValidationMessage, SiteValidationMessage)]
    [InlineData("sc.local", false, true, 1, "", SiteValidationMessage)]
    [InlineData("sc.local", true, false, 1, AppPoolValidationMessage, "")]
    [InlineData("sc.local", false, false, 0, "", "")]
    public void EvaluateTests(string taskSiteName, bool appPoolExists, bool siteExists, int errorsCount, string appPoolValidationMessage, string siteValidationMessage)
    {
      // Arrange 
      var fixture = new Fixture();

      var task = Substitute.For<Task>(TaskName, fixture.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
      InstallParam siteNameParam = new InstallParam(SiteName, taskSiteName);
      List<InstallParam> paramList = new List<InstallParam>
      {
        siteNameParam
      };
      task.LocalParams.Returns(paramList);

      var validator = Substitute.ForPartsOf<AppPoolSiteValidator>();
      validator.AppPoolExists(taskSiteName).Returns(appPoolExists);
      validator.SiteExists(taskSiteName).Returns(siteExists);

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] { task });
      IEnumerable<ValidationResult> errors = result.Where(r => r.State == ValidatorState.Error);

      // Assert
      Assert.Equal(errors.Count(), errorsCount);
      this.ValidateMessage(appPoolValidationMessage, taskSiteName, SiteName, task.Name, errors);
      this.ValidateMessage(siteValidationMessage, taskSiteName, SiteName, task.Name, errors);
    }

    private void ValidateMessage(string validationMessage, string paramValue, string paramName, string taskName, IEnumerable<ValidationResult> errors)
    {
      if (!string.IsNullOrEmpty(validationMessage))
      {
        validationMessage = string.Format(validationMessage, paramValue, paramName, taskName);
        Assert.Contains(errors, error => error.Message.Equals(validationMessage));
      }
    }
  }
}

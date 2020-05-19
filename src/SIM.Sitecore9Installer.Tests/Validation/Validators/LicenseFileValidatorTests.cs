using NSubstitute;
using SIM.Sitecore9Installer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class LicenseFileValidatorTests
  {
    [Theory]
    [InlineData(true, ValidatorState.Success)]
    [InlineData(false, ValidatorState.Error)]
    public void ReturnsValidValidationResults(bool fileExists, ValidatorState expectedResult)
    {
      //Arrange
      Task task = Substitute.For<Task>("", 0, null, new List<InstallParam>(), new Dictionary<string, string>());
      task.LocalParams.Returns(new List<InstallParam>());

      InstallParam licenseFileParam = new InstallParam("LicenseFile", @"C:\license.xml");
      task.LocalParams.Add(licenseFileParam);

      List<Task> tasks = Substitute.For<List<Task>>();
      tasks.Add(task);

      LicenseFileValidator val = Substitute.ForPartsOf<LicenseFileValidator>();
      val.FileExists(string.Empty).ReturnsForAnyArgs(fileExists);

      val.Data["LicenseFileVariable"] = "LicenseFile";

      //Act
      IEnumerable<ValidationResult> results = val.Evaluate(tasks);

      //Assert
      Assert.Contains(results, r => r.State == expectedResult);
    }
  }
}

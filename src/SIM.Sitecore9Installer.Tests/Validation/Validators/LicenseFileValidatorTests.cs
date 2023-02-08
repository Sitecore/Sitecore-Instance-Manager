using NSubstitute;
using SIM.Sitecore9Installer.Validation;
using System.Collections.Generic;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;
using System;
using System.Xml;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class LicenseFileValidatorTests
  {
    [Theory]
    [InlineData(true, ValidatorState.Success, false)]
    [InlineData(false, ValidatorState.Error, false)]
    [InlineData(true, ValidatorState.Warning, true)]
    [InlineData(true, ValidatorState.Error, true)]
    public void ReturnsValidValidationResults(bool fileExists, ValidatorState expectedResult, bool useExpiredDate)
    {
      //Arrange
      Task task = Substitute.For<Task>("", 0, null, null, new Dictionary<string, string>());
      GlobalParameters globals = new GlobalParameters();
      task.LocalParams.Returns(new LocalParameters(new List<InstallParam>(), globals));

      task.LocalParams.AddOrUpdateParam("LicenseFile", @"C:\license.xml", InstallParamType.String);

      List<Task> tasks = Substitute.For<List<Task>>();
      tasks.Add(task);

      LicenseFileValidator val = Substitute.ForPartsOf<LicenseFileValidator>();
      val.FileExists(string.Empty).ReturnsForAnyArgs(fileExists);
      val.GetXmlFileNodes(string.Empty, string.Empty).ReturnsForAnyArgs((XmlNodeList)null);
      val.Data["LicenseFileVariable"] = "LicenseFile";

      if (useExpiredDate)
      {
        if (expectedResult == ValidatorState.Error)
        {
          val.ExpiredDatesAndLicences.Add(DateTime.Now.AddYears(-1).ToShortDateString(), new List<string>() { "SiteCore.License" });
        }
        else if (expectedResult == ValidatorState.Warning)
        {
          val.AlmostExpiredDatesAndLicences.Add(DateTime.Now.AddMonths(-1).ToShortDateString(), new List<string>() { "SiteCore.License" });
        }
      }

      //Act
      IEnumerable<ValidationResult> results = val.Evaluate(tasks);

      //Assert
      Assert.Contains(results, r => r.State == expectedResult);
    }
  }
}

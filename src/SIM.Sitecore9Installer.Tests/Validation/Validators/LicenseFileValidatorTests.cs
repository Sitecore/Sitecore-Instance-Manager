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
    private const string LicenseFileName = "LicenseFile";
    private const string LicenseFilePath = @"C:\license.xml";
    private const string DateFormat = "yyyyMMdd";

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

      task.LocalParams.AddOrUpdateParam(LicenseFileName, LicenseFilePath, InstallParamType.String);

      List<Task> tasks = Substitute.For<List<Task>>();
      tasks.Add(task);

      LicenseFileValidator val = Substitute.ForPartsOf<LicenseFileValidator>();
      val.FileExists(string.Empty).ReturnsForAnyArgs(fileExists);

      string date = DateTime.Now.AddYears(1).ToString(DateFormat);
      if (useExpiredDate)
      {
        if (expectedResult == ValidatorState.Error)
        {
          date = DateTime.Now.AddYears(-1).ToString(DateFormat);
        }
        else if (expectedResult == ValidatorState.Warning)
        {
          date = DateTime.Now.AddMonths(1).ToString(DateFormat);
        }
      }

      string license = "<?xml version=\"1.0\" encoding=\"utf-16\"?><?xml-stylesheet type=\"text/xsl\" href=\"http://www.sitecore.net/licenseviewer/license.xsl\"?>" +
        "<signedlicense id=\"00000000000000\"><Signature><Object Id=\"SiteCore.License\" xmlns=\"http://www.w3.org/2000/09/xmldsig#\">" +
        $"<license xmlns=\"\"><expiration>{date}T000000</expiration></license></Object></Signature></signedlicense>";
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(license);
      val.GetXmlFileNodes(string.Empty, string.Empty).ReturnsForAnyArgs(xmlDocument.GetElementsByTagName("expiration"));
      val.Data["LicenseFileVariable"] = LicenseFileName;

      //Act
      IEnumerable<ValidationResult> results = val.Evaluate(tasks);

      //Assert
      Assert.Contains(results, r => r.State == expectedResult);
    }
  }
}

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
  public class CmDdsPatchSiteNameValidatorTests
  {
    private const string SitecoreXp1Cm = "sitecore-xp1-cm";

    private const string SitecoreXp1CmDdsPatch = "sitecore-XP1-cm-dds-patch";

    private const string SiteName = "SiteName";

    [Theory]
    [InlineData(SitecoreXp1Cm, "test", SitecoreXp1CmDdsPatch, "test", false)]
    [InlineData(SitecoreXp1Cm, "test", SitecoreXp1CmDdsPatch, "TEST", false)]
    [InlineData(SitecoreXp1Cm, "test", SitecoreXp1CmDdsPatch, "CM.local", true)]
    [InlineData(SitecoreXp1Cm, "CM.local", SitecoreXp1CmDdsPatch, "test", true)]
    [InlineData("unknown", "test", SitecoreXp1CmDdsPatch, "CM.local", false)]
    [InlineData(SitecoreXp1Cm, "CM.local", "unknown", "test", false)]
    public void EvaluateTests(string cmTaskName, string cmSiteName, string ddsPatchTaskName, string ddsPatchSiteName, bool areAnyErrors)
    {
      // Arrange 
      var fixture = new Fixture();

      var cmTask = Substitute.For<Task>(cmTaskName, fixture.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
      InstallParam siteNameParam = new InstallParam(SiteName, cmSiteName);
      List<InstallParam> paramList = new List<InstallParam>
      {
        siteNameParam
      };
      cmTask.LocalParams.Returns(paramList);

      var ddsPatchTask = Substitute.For<Task>(ddsPatchTaskName, fixture.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
      siteNameParam = new InstallParam(SiteName, ddsPatchSiteName);
      paramList = new List<InstallParam>
      {
        siteNameParam
      };
      ddsPatchTask.LocalParams.Returns(paramList);

      var validator = Substitute.ForPartsOf<CmDdsPatchSiteNameValidator>();
      validator.Data["SitecoreXp1Cm"] = SitecoreXp1Cm;
      validator.Data["SitecoreXp1CmDdsPatch"] = SitecoreXp1CmDdsPatch;
      validator.Data["SiteName"] = SiteName;

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] { cmTask, ddsPatchTask });
      IEnumerable<ValidationResult> errors = result.Where(r => r.State == ValidatorState.Error);

      // Assert
      Assert.Equal(errors.Any(), areAnyErrors);
    }
  }
}

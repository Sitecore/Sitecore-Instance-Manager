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
  public class CmIdentityServerSiteNameValidatorTests
  {
    private const string SitecoreXp1Cm = "sitecore-xp1-cm";

    private const string SitecoreXm1Cm = "Sitecore-xm1-cm";

    private const string SitecoreXp0 = "Sitecore-XP0";

    private const string SiteName = "SiteName";

    private const string IdentityServer = "IdentityServer";

    private const string AllowedCorsOrigins = "AllowedCorsOrigins";

    private const string PasswordRecoveryUrl = "PasswordRecoveryUrl";

    [Theory]
    [InlineData(SitecoreXp1Cm, "test", IdentityServer, "https://test", "http://test", 0)]
    [InlineData(SitecoreXm1Cm, "CM.local", IdentityServer, "https://CM.local", "https://CM.local", 0)]
    [InlineData(SitecoreXp0, "new", IdentityServer, "http://new", "https://new", 0)]
    [InlineData("unknown", "CM.local", IdentityServer, "https://cm.local", "https://CM.LOCAL", 0)]
    [InlineData(SitecoreXp1Cm, "test", "unknown", "https://CM.local", "https://CM.local", 0)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "https://test", "https://CM.local", 1)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "https://CM.local", "https://test", 1)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "http://CM.local", "http://CM.local", 2)]
    public void EvaluateTests(string cmTaskName, string cmSiteName, string identityServerTaskName, string identityServerAllowedCorsOrigins, string identityServerPasswordRecoveryUrl, int warningsCount)
    {
      // Arrange 
      var fixture = new Fixture();
      GlobalParameters globals = new GlobalParameters();
      var cmTask = Substitute.For<Task>(cmTaskName, fixture.Create<int>(), null, null, new Dictionary<string, string>());
      InstallParam siteNameParam = new InstallParam(SiteName, cmSiteName,false,InstallParamType.String);
      List<InstallParam> paramList = new List<InstallParam>
      {
        siteNameParam
      };
      LocalParameters cmLocals = new LocalParameters(paramList, globals);

      cmTask.LocalParams.Returns(cmLocals);

      var identityServerTask = Substitute.For<Task>(identityServerTaskName, fixture.Create<int>(), null, null, new Dictionary<string, string>());
      InstallParam allowedCorsOriginsParam = new InstallParam(AllowedCorsOrigins, identityServerAllowedCorsOrigins,false,InstallParamType.String);
      InstallParam passwordRecoveryUrlParam = new InstallParam(PasswordRecoveryUrl, identityServerPasswordRecoveryUrl,false, InstallParamType.String);
      paramList = new List<InstallParam>
      {
        allowedCorsOriginsParam,
        passwordRecoveryUrlParam
      };
      LocalParameters idLocals = new LocalParameters(paramList, globals);
      identityServerTask.LocalParams.Returns(idLocals);

      var validator = Substitute.ForPartsOf<CmIdentityServerSiteNameValidator>();
      validator.Data["SitecoreXp1Cm"] = SitecoreXp1Cm;
      validator.Data["SitecoreXm1Cm"] = SitecoreXm1Cm;
      validator.Data["SitecoreXp0"] = SitecoreXp0;
      validator.Data["SiteName"] = SiteName;
      validator.Data["IdentityServer"] = IdentityServer;
      validator.Data["AllowedCorsOrigins"] = AllowedCorsOrigins;
      validator.Data["PasswordRecoveryUrl"] = PasswordRecoveryUrl;

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] { cmTask, identityServerTask });
      IEnumerable<ValidationResult> warnings = result.Where(r => r.State == ValidatorState.Warning);

      // Assert
      Assert.Equal(warnings.Count(), warningsCount);
    }
  }
}

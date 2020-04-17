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

    private const string SitecoreXm0 = "Sitecore-XP0";

    private const string SiteName = "SiteName";

    private const string IdentityServer = "IdentityServer";

    private const string AllowedCorsOrigins = "AllowedCorsOrigins";

    private const string PasswordRecoveryUrl = "PasswordRecoveryUrl";

    [Theory]
    [InlineData(SitecoreXp1Cm, "test", IdentityServer, "https://test", "http://test", 0)]
    [InlineData(SitecoreXm1Cm, "CM.local", IdentityServer, "https://CM.local", "https://CM.local", 0)]
    [InlineData(SitecoreXm0, "new", IdentityServer, "http://new", "https://new", 0)]
    [InlineData("unknown", "CM.local", IdentityServer, "https://cm.local", "https://CM.LOCAL", 0)]
    [InlineData(SitecoreXp1Cm, "test", "unknown", "https://CM.local", "https://CM.local", 0)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "https://test", "https://CM.local", 1)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "https://CM.local", "https://test", 1)]
    [InlineData(SitecoreXm1Cm, "test", IdentityServer, "http://CM.local", "http://CM.local", 2)]
    public void EvaluateTests(string cmTaskName, string cmSiteName, string identityServerTaskName, string identityServerAllowedCorsOrigins, string identityServerPasswordRecoveryUrl, int warningsCount)
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

      var identityServerTask = Substitute.For<Task>(identityServerTaskName, fixture.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
      InstallParam allowedCorsOriginsParam = new InstallParam(AllowedCorsOrigins, identityServerAllowedCorsOrigins);
      InstallParam passwordRecoveryUrlParam = new InstallParam(PasswordRecoveryUrl, identityServerPasswordRecoveryUrl);
      paramList = new List<InstallParam>
      {
        allowedCorsOriginsParam,
        passwordRecoveryUrlParam
      };
      identityServerTask.LocalParams.Returns(paramList);

      var validator = Substitute.ForPartsOf<CmIdentityServerSiteNameValidator>();

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] { cmTask, identityServerTask });
      IEnumerable<ValidationResult> warnings = result.Where(r => r.State == ValidatorState.Warning);

      // Assert
      Assert.Equal(warnings.Count(), warningsCount);
    }
  }
}

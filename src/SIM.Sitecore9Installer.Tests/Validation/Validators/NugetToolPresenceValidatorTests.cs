using NSubstitute;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Linq;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class NugetToolPresenceValidatorTests : NugetToolPresenceValidator
  {
    [Theory]
    [InlineData("somenonexistingcommand-9583457661d0")]
    public void GetScriptError_Should_ReturnError(string command)
    {
      // PS script should throw the "CommandNotFoundException" error since we are running non-existing command.

      // Arrange
      string script = command;
      NugetToolPresenceValidatorTests validator = new NugetToolPresenceValidatorTests();

      // Act
      string result = validator.GetScriptError(script);

      // Assert
      Assert.Equal("CommandNotFoundException", result);
    }

    [Theory]
    [InlineData("Get-Location")]
    public void GetScriptError_ShouldNot_ReturnError(string command)
    {
      // PS scripts should not throw any error since we are running well known('Get-Location') command.

      // Arrange
      string script = command;
      NugetToolPresenceValidatorTests validator = new NugetToolPresenceValidatorTests();

      // Act
      string result = validator.GetScriptError(script);

      // Assert
      Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetValidationResults_SholdNotReturn_ErrorValidationResult()
    {
      // Arrange
      var validator = Substitute.ForPartsOf<NugetToolPresenceValidatorTests>();
      validator.When(a => a.GetScript(Arg.Any<string>())).DoNotCallBase();
      validator.When(a => a.GetScriptError(Arg.Any<string>())).DoNotCallBase();
      validator.GetScriptError(Arg.Any<string>()).Returns(string.Empty);

      // Act
      var result = validator.GetValidationResults();

      // Assert
      Assert.False(result.Any());
    }

    [Fact]
    public void GetValidationResults_ShouldReturn_ErrorValidationResult()
    {
      // Arrange
      var validator = Substitute.ForPartsOf<NugetToolPresenceValidatorTests>();
      validator.When(a => a.GetScript(Arg.Any<string>())).DoNotCallBase();
      validator.When(a => a.GetScriptError(Arg.Any<string>())).DoNotCallBase();
      validator.GetScriptError(Arg.Any<string>()).Returns("CommandNotFoundException");

      // Act
      var result = validator.GetValidationResults();

      // Assert
      Assert.True(result.Any());
    }
  }
}
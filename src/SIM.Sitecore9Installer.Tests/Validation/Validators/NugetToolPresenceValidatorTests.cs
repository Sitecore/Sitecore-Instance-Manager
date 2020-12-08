using NSubstitute;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Collections.Generic;
using System.Linq;
using SIM.Sitecore9Installer.Tasks;
using Xunit;
using SIM.Sitecore9Installer.Validation;
using AutoFixture;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class NugetToolPresenceValidatorTests
  {
    [Theory]
    [InlineData("somenonexistingcommand-9583457661d0", true)]
    [InlineData("Get-Location", false)]
    public void EvaluateTests(string commandToEvaluate, bool areAnyErrors)
    {
      // PS script should throw the "CommandNotFoundException" error since we are running non-existing command.

      // Arrange 
      var fixture = new Fixture();
      var task = Substitute.For<Task>("sitecore-XP1-cm-dds-patch", fixture.Create<int>(), null, null, new Dictionary<string, string>());
      GlobalParameters globals = new GlobalParameters();
      task.GlobalParams.Returns(globals);
      task.LocalParams.Returns(new LocalParameters(new List<InstallParam>(),globals));
      var validator = Substitute.ForPartsOf<NugetToolPresenceValidator>();
      validator.CommandToEvaluate.Returns(commandToEvaluate);

      // Act
      IEnumerable<ValidationResult> result = validator.Evaluate(new Task[] { task });
      IEnumerable<ValidationResult> errors = result.Where(r => r.State == ValidatorState.Error);

      // Assert
      Assert.Equal(errors.Any(), areAnyErrors);
    }
  }
}
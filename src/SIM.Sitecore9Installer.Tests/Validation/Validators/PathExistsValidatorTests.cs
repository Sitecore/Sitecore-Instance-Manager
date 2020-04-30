using AutoFixture.Xunit2;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class PathExistsValidatorTests
  {
    private PathExistsValidator CreateValidator(string paramNames, bool pathExists)
    {
      PathExistsValidator val = Substitute.ForPartsOf<PathExistsValidator>();
      val.WhenForAnyArgs(v => v.PathExists(null)).DoNotCallBase();
      val.PathExists(null).ReturnsForAnyArgs(pathExists);
      val.Data["ParamNames"] = paramNames;
      return val;
    }
    [Theory]
    [AutoData]
    public void PathExists(string pathName, string pathValue)
    {      
      PathExistsValidator val = this.CreateValidator(pathName, true);
      Task task = ValidatorTestSetup.CreateTask("someTask", new string[] { pathName }, new string[] { pathValue });
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { task });
      Assert.DoesNotContain(results, r => r.State == ValidatorState.Error);
      Assert.Contains(results, r => r.State == ValidatorState.Success);
    }

    [Theory]
    [AutoData]
    public void PathNotExist(string pathName, string pathValue)
    {
      PathExistsValidator val = this.CreateValidator(pathName, false);
      Task task = ValidatorTestSetup.CreateTask("someTask", new string[] { pathName }, new string[] { pathValue });
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { task });
      Assert.Contains(results, r => r.State == ValidatorState.Error);
      Assert.DoesNotContain(results, r => r.State == ValidatorState.Success);
    }
  }
}

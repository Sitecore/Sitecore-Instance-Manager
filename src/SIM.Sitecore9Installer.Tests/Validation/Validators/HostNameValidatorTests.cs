using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class HostNameValidatorTests
  {
    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void HostNameIsValid(IEnumerable<Task> tasks)
    {
      foreach(Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("DnsName", "test.com",InstallParamType.String);
      }

      HostNameValidator val = new HostNameValidator();
      val.Data["ParamNames"] = "DnsName";
      IEnumerable<ValidationResult> res= val.Evaluate(tasks);
      int count = res.Count(r => r.State != ValidatorState.Success);
      Assert.Equal(0, count);
    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void HostNameIsInvalid(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("DnsName", "!@#$%%",InstallParamType.String);
      }

      HostNameValidator val = new HostNameValidator();
      val.Data["ParamNames"] = "DnsName";
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      int count = res.Count(r => r.State == ValidatorState.Error);
      Assert.Equal(tasks.Count(), count);
    }
  }
}

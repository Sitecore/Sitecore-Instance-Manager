using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class SolrServiceValidatorTests
  {
    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void ServiceDoesNotExist(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("SolrService", "nope",InstallParamType.String);
      }

      SolrServiceValidator val = Substitute.ForPartsOf<SolrServiceValidator>();
      val.Data["ParamNames"] = Guid.NewGuid().ToString();
      ServiceControllerWrapper s = null;
      val.GetService(Arg.Any<string>()).ReturnsForAnyArgs(s);
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      Assert.Equal(0, res.Count(r => r.State == ValidatorState.Error));
    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void ServiceIsRunning(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("SolrService", "nope",InstallParamType.String);
      }

      SolrServiceValidator val = Substitute.ForPartsOf<SolrServiceValidator>();
      val.Data["ParamNames"] = Guid.NewGuid().ToString();
      ServiceController c = null;
      ServiceControllerWrapper s = Substitute.For<ServiceControllerWrapper>(c);
      s.Status.Returns(ServiceControllerStatus.Running);
      val.GetService(Arg.Any<string>()).ReturnsForAnyArgs(s);
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      Assert.Equal(0, res.Count(r => r.State == ValidatorState.Error));
    }
  }
}

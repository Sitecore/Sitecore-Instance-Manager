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
    public void ServiceIsStopped(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("SolrUrl", "https://localhost:8983/solr", InstallParamType.String);
      }

      SolrServiceValidator val = Substitute.ForPartsOf<SolrServiceValidator>();
      SolrStateResolver solrStateResolver = Substitute.ForPartsOf<SolrStateResolver>();
      ServiceController serviceController = Substitute.For<ServiceController>();
      ServiceControllerWrapper serviceControllerWrapper = Substitute.For<ServiceControllerWrapper>(serviceController);
      solrStateResolver.GetService(Arg.Any<string>()).ReturnsForAnyArgs(serviceControllerWrapper);
      solrStateResolver.GetServiceState(Arg.Any<ServiceControllerWrapper>()).ReturnsForAnyArgs(SolrState.CurrentState.Stopped);
      val.SolrStateResolver.Returns(solrStateResolver);
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      Assert.Equal(2, res.Count(r => r.State == ValidatorState.Error));
    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void ServiceIsRunning(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.AddOrUpdateParam("SolrUrl", "https://localhost:8983/solr", InstallParamType.String);
      }

      SolrServiceValidator val = Substitute.ForPartsOf<SolrServiceValidator>();
      SolrStateResolver solrStateResolver = Substitute.ForPartsOf<SolrStateResolver>();
      ServiceController serviceController = Substitute.For<ServiceController>();
      ServiceControllerWrapper serviceControllerWrapper = Substitute.For<ServiceControllerWrapper>(serviceController);
      solrStateResolver.GetService(Arg.Any<string>()).ReturnsForAnyArgs(serviceControllerWrapper);
      solrStateResolver.GetServiceState(Arg.Any<ServiceControllerWrapper>()).ReturnsForAnyArgs(SolrState.CurrentState.Running);
      val.SolrStateResolver.Returns(solrStateResolver);
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      Assert.Equal(0, res.Count(r => r.State == ValidatorState.Error));
    }
  }
}

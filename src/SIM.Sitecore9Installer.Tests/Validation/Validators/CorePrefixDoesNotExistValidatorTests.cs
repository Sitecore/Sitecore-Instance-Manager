using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Collections.Generic;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class CorePrefixDoesNotExistValidatorTests
  {
    private CorePrefixDoesNotExistValidator CreateValidator(string prefixValue, string PrefixName, string solrValue, string solrName, IEnumerable<string> coresToReturn)
    {      
      CorePrefixDoesNotExistValidator val = Substitute.ForPartsOf<CorePrefixDoesNotExistValidator>();
      val.Data["Prefix"] = PrefixName;
      val.Data["Solr"] = solrName;
      val.WhenForAnyArgs(v => v.GetCores(string.Empty)).DoNotCallBase();
      val.GetCores(null).ReturnsForAnyArgs(coresToReturn);
      return val;
    }   

    [Theory]
    [AutoData]
    public void CoreWithTheSamePrefixExists(string prefixValue, string PrefixName, string solrValue, string solrName )
    {
      //setup
      CorePrefixDoesNotExistValidator val = this.CreateValidator(prefixValue, PrefixName, solrValue, solrName, new string[] {prefixValue+"_core" });
      Task task = ValidatorTestSetup.CreateTask("someTask", new string[] { PrefixName, solrName }, new string[] { prefixValue, solrValue });
      //act
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { task });
      Assert.Contains(results, r => r.State == ValidatorState.Error);
      Assert.DoesNotContain(results, r => r.State == ValidatorState.Success);
    }

    [Theory]
    [AutoData]
    public void CoreWithTheSamePrefixDoesNotExist(string prefixValue, string PrefixName, string solrValue, string solrName, string[] coresToReturn)
    {
      //setup
      CorePrefixDoesNotExistValidator val = this.CreateValidator(prefixValue, PrefixName, solrValue, solrName, coresToReturn);
      Task task = ValidatorTestSetup.CreateTask("someTask", new string[] { PrefixName, solrName }, new string[] { prefixValue, solrValue });
      //act
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { task });
      Assert.Contains(results, r => r.State == ValidatorState.Success);
      Assert.DoesNotContain(results, r => r.State == ValidatorState.Error);
    }
  }
}

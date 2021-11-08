using AutoFixture;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class SqlPefixValidatorTests
  {
    [Theory]
    [InlineData("server","user","pass","prefix","sql-server","sql-user","sql-pass","db-prefix",new string[] {"somedb"},1)]
    [InlineData("server", "user", "pass", "prefix", "sql-server", "sql-user", "sql-pass", "db-prefix", new string[] { }, 0)]
    public void DbsWithTheSamePrefix(string server, string user, string pass, string prefix, 
      string serverValue, string userValue, string passValue, string prefixValue, string[] foundDbs, int errorsCount)
    {
      SqlPefixValidator val = Substitute.ForPartsOf<SqlPefixValidator>();
      val.WhenForAnyArgs(v => v.GetDbList(null, null, null, null)).DoNotCallBase();
      val.GetDbList(null, null, null, null).ReturnsForAnyArgs(foundDbs);      
      val.Data["Server"] = server;
      val.Data["User"] = user;
      val.Data["Password"] = pass;
      val.Data["Prefix"] = prefix;
      Fixture _fix = new Fixture();
      Task t = Substitute.For<Task>(_fix.Create<string>(), _fix.Create<int>(), null, null, new Dictionary<string, string>());
      GlobalParameters globals = new GlobalParameters();
      t.GlobalParams.Returns(globals);
      t.LocalParams.Returns(new LocalParameters(new List<InstallParam>(),globals));
      t.LocalParams.AddOrUpdateParam(server, serverValue,InstallParamType.String);
      t.LocalParams.AddOrUpdateParam(user, userValue,InstallParamType.String);
      t.LocalParams.AddOrUpdateParam(pass, passValue,InstallParamType.String);
      t.LocalParams.AddOrUpdateParam(prefix, prefixValue,InstallParamType.String);
      //act
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { t});
      Assert.Equal(errorsCount,results.Count(r => r.State == Sitecore9Installer.Validation.ValidatorState.Error));
    }  
  }
}

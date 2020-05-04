using AutoFixture;
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
      Task t = Substitute.For<Task>(_fix.Create<string>(), _fix.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
      t.GlobalParams.Returns(new List<InstallParam>());
      t.LocalParams.Returns(new List<InstallParam>());
      t.LocalParams.Add(new InstallParam(server, serverValue));
      t.LocalParams.Add(new InstallParam(user, userValue));
      t.LocalParams.Add(new InstallParam(pass, passValue));
      t.LocalParams.Add(new InstallParam(prefix, prefixValue));
      //act
      IEnumerable<ValidationResult> results = val.Evaluate(new Task[] { t});
      Assert.Equal(errorsCount,results.Count(r => r.State == Sitecore9Installer.Validation.ValidatorState.Error));
    }  
  }
}

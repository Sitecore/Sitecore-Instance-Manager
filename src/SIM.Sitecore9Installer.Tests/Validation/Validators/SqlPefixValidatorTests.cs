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
    [ClassData(typeof(ValidatorTestSetup))]
    public void NoDbsWithTheSamePrefix(IEnumerable<Task> tasks)
    {
      SqlPefixValidator val = Substitute.ForPartsOf<SqlPefixValidator>();
      val.WhenForAnyArgs(v => v.GetDbList(null, null, null)).DoNotCallBase();
      val.GetDbList(null, null, null).ReturnsForAnyArgs(new string[] { "someDb" });
      string server = "SqlServer";
      string user = "SqlAdminUser";
      string pass = "SqlAdminPassword";
      string prefix = "SqlDbPrefix";
      val.Data["Server"] = server;
      val.Data["User"] = user;
      val.Data["Password"] = pass;
      val.Data["Prefix"] = prefix;
      Task t = tasks.First();
      t.LocalParams.Add(new InstallParam(server, "sql-server"));
      t.LocalParams.Add(new InstallParam(user, "sql-user"));
      t.LocalParams.Add(new InstallParam(pass, "sql-pass"));
      t.LocalParams.Add(new InstallParam(prefix, "sql-prefix"));

      IEnumerable<ValidationResult> results = val.Evaluate(tasks);
      Assert.DoesNotContain(results, r => r.State != Sitecore9Installer.Validation.ValidatorState.Success);

    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void DbsWithTheSamePrefix(IEnumerable<Task> tasks)
    {
      SqlPefixValidator val = Substitute.ForPartsOf<SqlPefixValidator>();
      val.WhenForAnyArgs(v => v.GetDbList(null, null, null)).DoNotCallBase();      
      string server = "SqlServer";
      string user = "SqlAdminUser";
      string pass = "SqlAdminPassword";
      string prefix = "SqlDbPrefix";
      val.Data["Server"] = server;
      val.Data["User"] = user;
      val.Data["Password"] = pass;
      val.Data["Prefix"] = prefix;
      Task t = tasks.First();
      t.LocalParams.Add(new InstallParam(server, "sql-server"));
      t.LocalParams.Add(new InstallParam(user, "sql-user"));
      t.LocalParams.Add(new InstallParam(pass, "sql-pass"));
      t.LocalParams.Add(new InstallParam(prefix, "sql-prefix"));
      val.GetDbList(null, null, null).ReturnsForAnyArgs(new string[] { "sql-prefixDatabase","SomeOtherDb" });
      IEnumerable<ValidationResult> results = val.Evaluate(tasks);
      Assert.Contains(results, r => r.State == Sitecore9Installer.Validation.ValidatorState.Error);

    }
  }
}

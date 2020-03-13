using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class SqlVersionValidatorTests
  {
    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void Compatible(IEnumerable<Task> tasks)
    {
      InstallParam server = new InstallParam("server", "val");
      InstallParam user = new InstallParam("user", "val");
      InstallParam pass = new InstallParam("pass", "val");
      Task t= tasks.First();
      t.LocalParams.Add(server);
      t.LocalParams.Add(user);
      t.LocalParams.Add(pass);

      SqlVersionValidator val = Substitute.ForPartsOf<SqlVersionValidator>();
      val.Data["Server"] = "server";
      val.Data["User"]="user";
      val.Data["Password"]="pass";
      val.Data["Versions"] = "15.0.*,17.0.*";
      val.WhenForAnyArgs(v => v.GetSqlVersion(null, null, null)).DoNotCallBase();
      val.GetSqlVersion(null, null, null).ReturnsForAnyArgs("15.0.12345.6789");
      Assert.DoesNotContain(val.Evaluate(tasks),r=>r.State!=Sitecore9Installer.Validation.ValidatorState.Success);
    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void NotCompatible(IEnumerable<Task> tasks)
    {
      InstallParam server = new InstallParam("server", "val");
      InstallParam user = new InstallParam("user", "val");
      InstallParam pass = new InstallParam("pass", "val");
      Task t = tasks.First();
      t.LocalParams.Add(server);
      t.LocalParams.Add(user);
      t.LocalParams.Add(pass);

      SqlVersionValidator val = Substitute.ForPartsOf<SqlVersionValidator>();
      val.Data["Server"] = "server";
      val.Data["User"] = "user";
      val.Data["Password"] = "pass";
      val.Data["Versions"] = "15.0.*,17.0.*";
      val.WhenForAnyArgs(v => v.GetSqlVersion(null, null, null)).DoNotCallBase();
      val.GetSqlVersion(null, null, null).ReturnsForAnyArgs("12.0.12345.6789");
      Assert.Contains(val.Evaluate(tasks), r => r.State == Sitecore9Installer.Validation.ValidatorState.Error);

    }

  }
  
}

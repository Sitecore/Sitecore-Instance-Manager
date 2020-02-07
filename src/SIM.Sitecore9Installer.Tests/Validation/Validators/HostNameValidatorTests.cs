using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class HostNameValidatorTests
  {
    static Fixture _fix;
    public static Fixture Fix
    {
      get
      {
        if (_fix == null)
        {
          _fix = new Fixture();
          _fix.Register<Task>(() =>
          {
            Task t = Substitute.For<Task>(_fix.Create<string>(), _fix.Create<int>(), null, new List<InstallParam>(), new Dictionary<string, string>());
            t.GlobalParams.Returns(new List<InstallParam>());
            t.LocalParams.Returns(new List<InstallParam>());
            return t;
          });
        }

        return _fix;
      }
    }

    [Theory]
    [ClassData(typeof(TasksData))]
    public void HostNameIsValid(IEnumerable<Task> tasks)
    {
      foreach(Task t in tasks)
      {
        t.LocalParams.Add(new InstallParam("DnsName", "test.com"));
      }

      HostNameValidator val = new HostNameValidator();
      IEnumerable<ValidationResult> res= val.Evaluate(tasks);
      int count = res.Count(r => r.State != ValidatorState.Success);
      Assert.Equal(0, count);
    }

    [Theory]
    [ClassData(typeof(TasksData))]
    public void HostNameIsInvalid(IEnumerable<Task> tasks)
    {
      foreach (Task t in tasks)
      {
        t.LocalParams.Add(new InstallParam("DnsName", "!@#$%%"));
      }

      HostNameValidator val = new HostNameValidator();
      IEnumerable<ValidationResult> res = val.Evaluate(tasks);
      int count = res.Count(r => r.State == ValidatorState.Error);
      Assert.Equal(tasks.Count(), count);
    }
  }

  public class TasksData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      yield return new object[] { HostNameValidatorTests.Fix.CreateMany<Task>(2) };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}

//provide test setup common for all validators.
using AutoFixture;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class ValidatorTestSetup: IEnumerable<object[]>
  {
    static Fixture _fix;
    public static Fixture Fix
    {
      get
      {
        if (_fix == null)
        {
          _fix = new Fixture();
          _fix.Register<Tasks.Task>(() =>
          {
            Task t = Substitute.For<Task>(_fix.Create<string>(), _fix.Create<int>(), null, null, new Dictionary<string, string>());
            GlobalParameters global = new GlobalParameters();
            t.GlobalParams.Returns(global);
            t.LocalParams.Returns(new LocalParameters(new List<InstallParam>(), global));
            return t;
          });
        }

        return _fix;
      }
    }

    public IEnumerator<object[]> GetEnumerator()
    {
      yield return new object[] { Fix.CreateMany<Task>(2) };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public static Task CreateTask(string taskName, string[] paramNames, string[] paramValues)
    {
      var task = Substitute.For<Task>("someTask", 1, null, null,
        new Dictionary<string, string>());
      List<InstallParam> iParams = new List<InstallParam>();
      for (int i = 0; i < paramNames.Length; ++i)
      {
        InstallParam p = new InstallParam(paramNames[i], paramValues[i],false,InstallParamType.String);
        iParams.Add(p);
      }

      task.LocalParams.Returns(new LocalParameters(iParams,new GlobalParameters()));
      return task;
    }
  }
}

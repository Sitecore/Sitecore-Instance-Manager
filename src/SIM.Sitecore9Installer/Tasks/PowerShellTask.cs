using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Newtonsoft.Json;

namespace SIM.Sitecore9Installer.Tasks
{
  public enum TaskState
  {
    Pending,
    Running,
    Finished,
    Warning,
    Failed
  }

  public abstract class PowerShellTask : Task
  {
    public PowerShellTask(string taskName, int executionOrder, Tasker owner, List<InstallParam> localParams,
      Dictionary<string, string> taskOptions)
      : base(taskName, executionOrder, owner, localParams, taskOptions)
    {
    }

    public override string Run()
    {
      var results = new StringBuilder();
      State = TaskState.Running;
      using (var PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(GetScript());
        try
        {
          PowerShellInstance.Invoke();
        }
        catch (Exception ex)
        {
          State = TaskState.Failed;
          return ex.ToString();
        }

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          foreach (var error in PowerShellInstance.Streams.Error)
          {
            var target = error.TargetObject as PSCmdlet;
            if (target != null)
            {
              var param = (ActionPreference) target.MyInvocation.BoundParameters["ErrorAction"];
              if (param == ActionPreference.Continue) continue;
            }

            results.AppendLine(error.ToString());
          }

          State = results.Length > 0 ? TaskState.Warning : TaskState.Finished;
        }
        else
        {
          State = TaskState.Finished;
        }
      }

      return results.ToString();
    }

    public abstract string GetScript();

    public virtual string GetSerializedParameters()
    {
      return GetSerializedParameters(new string[0]);
    }

    public virtual string GetSerializedParameters(IEnumerable<string> excludeList)
    {
      if (LocalParams.Count == 0) return null;

      return JsonConvert.SerializeObject(LocalParams.Where(p => !excludeList.Contains(p.Name)));
    }
  }
}
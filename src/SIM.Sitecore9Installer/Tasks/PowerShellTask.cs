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
    public PowerShellTask(string taskName, int executionOrder, List<InstallParam> globalParams, List<InstallParam> localParams,
      Dictionary<string, string> taskOptions, IParametersHandler handler)
      : base(taskName, executionOrder, globalParams,localParams, taskOptions, handler)
    {
    }

    public override string Run()
    {
      StringBuilder results = new StringBuilder();
      State = TaskState.Running;      
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        try
        {
          string script = this.GetEvaluatedScript();
          PowerShellInstance.AddScript(script);
          PowerShellInstance.Invoke();
        }
        catch (Exception)
        {
          State = TaskState.Failed;
          throw;
        }

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          foreach (ErrorRecord error in PowerShellInstance.Streams.Error)
          {
            PSCmdlet target = error.TargetObject as PSCmdlet;
            if (target != null)
            {
              ActionPreference param = (ActionPreference) target.MyInvocation.BoundParameters["ErrorAction"];
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
    
    public virtual string GetSerializedParameters()
    {
      return GetSerializedParameters(new string[0]);
    }

    public virtual string GetEvaluatedScript()
    {
      this.EvaluateLocalParams();
      return this.GetScript();
    }

    public virtual string GetSerializedParameters(IEnumerable<string> excludeList)
    {
      if (LocalParams.Count == 0) return null;

      return JsonConvert.SerializeObject(LocalParams.Where(p => !excludeList.Contains(p.Name)));
    }

    protected virtual void EvaluateLocalParams()
    {
      this.ParamsHandler.EvaluateLocalParams(this.LocalParams, this.GlobalParams);
    }

    protected abstract string GetScript();
  }
}
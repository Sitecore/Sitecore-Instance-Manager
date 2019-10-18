using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SIM.Sitecore9Installer
{
  public enum TaskState
  {
    Pending, Running, Finished, Warning, Failed
  }

  public abstract class PowerShellTask
  {
    public PowerShellTask()
    {
      this.LocalParams = new List<InstallParam>();
      this.GlobalParams = new List<InstallParam>();
    }

    public TaskState State { get; protected set; }
    public string Name { get; protected set; }
    public bool ShouldRun { get; set; }
    public int ExecutionOrder { get; protected set; }
    public bool UnInstall { get; set; }
    public List<InstallParam> GlobalParams { get; set; }
    public List<InstallParam> LocalParams { get; set; }

    public virtual string Run()
    {
      StringBuilder results = new StringBuilder();
      this.State = TaskState.Running;
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(this.GetScript());
        try
        {
          PowerShellInstance.Invoke();
        }
        catch (Exception ex)
        {
          this.State = TaskState.Failed;
          return ex.ToString();
        }

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          foreach (ErrorRecord error in PowerShellInstance.Streams.Error)
          {
            var target = error.TargetObject as System.Management.Automation.PSCmdlet;
            if (target != null)
            {
              ActionPreference param = (ActionPreference) target.MyInvocation.BoundParameters["ErrorAction"];
              if (param == ActionPreference.Continue)
              {
                continue;
              }
            }

            results.AppendLine(error.ToString());
          }

          this.State = results.Length > 0 ? TaskState.Warning : TaskState.Finished;
        }
        else
        {
          this.State = TaskState.Finished;
        }
      }

      return results.ToString();
    }

    public virtual bool SupportsUninstall()
    {
      return false;
    }
    public abstract string GetScript();

    public virtual string GetSerializedParameters()
    {
      return this.GetSerializedParameters(new string[0]);
    }

    public virtual string GetSerializedParameters(IEnumerable<string> excludeList)
    {
      if (this.LocalParams.Count == 0)
      {
        return null;
      }

      return JsonConvert.SerializeObject(this.LocalParams.Where(p => !excludeList.Contains(p.Name)));
    }

  }
}

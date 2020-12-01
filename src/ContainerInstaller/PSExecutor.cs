using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace ContainerInstaller
{
  public class PSExecutor
  {
    private string root;
    private string script;

    public PSExecutor(string executionDir):this(string.Empty, executionDir)
    {}

    public PSExecutor(string script, string executionDir)
    {
      this.script = script;
      this.root = executionDir;
    }

    public Collection<PSObject> Execute()
    {
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript($"cd \"{this.root}\"\n");
        PowerShellInstance.AddScript(this.GetScript());
        Collection<PSObject> result= PowerShellInstance.Invoke();

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          foreach (ErrorRecord error in PowerShellInstance.Streams.Error)
          {
            PSCmdlet target = error.TargetObject as PSCmdlet;
            if (target != null)
            {
              ActionPreference param = (ActionPreference)target.MyInvocation.BoundParameters["ErrorAction"];
              if (param == ActionPreference.Continue)
              {
                continue;
              }

              throw new AggregateException($"Failed to execute script.\n{error.ToString()}");
            }
          }
        }

        return result;
      }
    }

    public virtual string GetScript()
    {
      return script;
    }
  }
}

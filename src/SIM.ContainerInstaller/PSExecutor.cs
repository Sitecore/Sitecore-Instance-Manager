using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace SIM.ContainerInstaller
{
  public abstract class PSExecutor
  {
    private string root;

    public PSExecutor(string executionDir)
    {
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

            if (error.FullyQualifiedErrorId.Equals("CommandNotFoundException",
              StringComparison.InvariantCultureIgnoreCase))
            {
              throw new CommandNotFoundException(error.Exception.Message);
            }
          }
        }

        return result;
      }
    }

    public abstract string GetScript();
  }
}

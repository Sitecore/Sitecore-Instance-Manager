using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Tasks
{
  public class EvaluateParamsTask : Task
  {
    public EvaluateParamsTask(string taskName, int executionOrder, GlobalParameters globalParams, 
      LocalParameters localParams, Dictionary<string, string> taskOptions) : base(taskName, executionOrder, globalParams, localParams, taskOptions)
    {
    }

    public override string Run()
    {
      try
      {
        this.GlobalParams.Evaluate();
      }
      catch
      {
        this.State = TaskState.Failed;
        throw;
      }

      this.State = TaskState.Finished;
      return string.Empty;
    }
  }
}

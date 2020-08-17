using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Tasks
{
  public class EvaluateParamsTask : Task
  {
    public EvaluateParamsTask(string taskName, int executionOrder, List<InstallParam> globalParams, 
      List<InstallParam> localParams, Dictionary<string, string> taskOptions, IParametersHandler handler) : base(taskName, executionOrder, globalParams, localParams, taskOptions, handler)
    {
    }

    public override string Run()
    {
      try
      {
        this.ParamsHandler.EvaluateGlobalParams(this.GlobalParams);
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

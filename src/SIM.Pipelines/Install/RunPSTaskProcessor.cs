using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer.Tasks;
using Sitecore.Diagnostics.Base;
using System;
using System.Linq;

namespace SIM.Pipelines.Install
{
  public class RunPSTaskProcessor : Processor
  {
    string taskName;
    public RunPSTaskProcessor(string TaskName)
    {
      Assert.ArgumentNotNullOrEmpty(TaskName, nameof(TaskName));
      this.taskName = TaskName;
      this.ProcessorDefinition = new SingleProcessorDefinition() { ProcessAlways = false };
    }

    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      Sitecore9Installer.Tasks.Task task =arguments.Tasker.Tasks.FirstOrDefault(t => t.Name == this.taskName);
      Assert.ArgumentNotNull(task, nameof(task));

      string result = string.Empty;
      try
      {
        result = task.Run();
      }
      catch (Exception ex)
      {
        this.HandleError(task, ex);
      }

      if (task.State != TaskState.Finished)
      {
        this.HandleError(task, result);
      }   
    }

    private void HandleError(Task task, Exception e)
    {
      this.SetLogLocation(task);
      throw new AggregateException(string.Format("Failed to execute {0} task.", task.Name), e);
    }

    private void HandleError(Task task, string failure)
    {
      this.SetLogLocation(task);
      throw new AggregateException(string.Format("Failed to execute {0} task. \n{1}", task.Name, failure));
    }

    private void SetLogLocation(Sitecore9Installer.Tasks.Task task)
    {
      SitecoreTask sTask = task as SitecoreTask;

      if (sTask != null)
      {
        this.CustomLogLocation = sTask.GlobalParams.First(p=>p.Name== "FilesRoot").Value;
      }
    }
  }
}

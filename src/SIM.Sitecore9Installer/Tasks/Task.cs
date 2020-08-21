using System.Collections.Generic;

namespace SIM.Sitecore9Installer.Tasks
{
  public abstract class Task
  {
    public Task(string taskName, int executionOrder, GlobalParameters globalParams, 
      LocalParameters localParams, Dictionary<string, string> taskOptions)
    {
      this.Name = taskName;      
      this.LocalParams = localParams;
      this.ShouldRun = true;
      this.TaskOptions = taskOptions;
      this.State = TaskState.Pending;
      this.GlobalParams = globalParams;
      this.InnerTasks = new List<SitecoreTask>();
      if (taskOptions.ContainsKey("ExecutionOrder"))
      {
        this.ExecutionOrder = int.Parse(taskOptions["ExecutionOrder"]);
      }
      else
      {
        this.ExecutionOrder = executionOrder;
      }
    }

    public TaskState State { get; protected set; }
    public string Name { get; protected set; }
    public bool ShouldRun { get; set; }
    public Dictionary<string, string> TaskOptions { get; }
    public int ExecutionOrder { get; protected set; }
    public bool UnInstall { get; set; }
    //public virtual Tasker Owner { get; }
    public BaseParameters GlobalParams { get; }
    public BaseParameters LocalParams { get; set; }
    public List<SitecoreTask> InnerTasks { get; }

    public abstract string Run();
    public virtual bool SupportsUninstall()
    {
      if (this.TaskOptions.TryGetValue("SupportsUninstall", out var value))
      {
        return bool.Parse(value);
      }

      return true;
    }
  }
}
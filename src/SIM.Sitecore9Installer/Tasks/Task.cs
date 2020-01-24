﻿using System.Collections.Generic;

namespace SIM.Sitecore9Installer.Tasks
{
  public abstract class Task
  {
    public Task(string taskName, int executionOrder, Tasker owner, List<InstallParam> localParams,
      Dictionary<string, string> taskOptions)
    {
      this.Name = taskName;
      this.ExecutionOrder = executionOrder;
      this.Owner = owner;
      this.LocalParams = localParams;
      this.ShouldRun = true;
      this.TaskOptions = taskOptions;
      this.State = TaskState.Pending;
      this.InnerTasks = new List<SitecoreTask>();
    }

    public TaskState State { get; protected set; }
    public string Name { get; protected set; }
    public bool ShouldRun { get; set; }
    public Dictionary<string, string> TaskOptions { get; }
    public int ExecutionOrder { get; protected set; }
    public bool UnInstall { get; set; }
    public Tasker Owner { get; }
    public List<InstallParam> GlobalParams => Owner.GlobalParams;
    public List<InstallParam> LocalParams { get; set; }
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
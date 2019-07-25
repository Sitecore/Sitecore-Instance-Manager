using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class GenerateScriptProcessor : Processor
  {

    string taskName;
    public GenerateScriptProcessor(string TaskName)
    {
      Assert.ArgumentNotNullOrEmpty(TaskName, nameof(TaskName));
      this.taskName = TaskName;
      this.ProcessorDefinition = new SingleProcessorDefinition() {ProcessAlways=false };
    }   

    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      SitecoreTask task = arguments.Tasker.Tasks.FirstOrDefault(t => t.Name == this.taskName);
      Assert.ArgumentNotNull(task, nameof(task));
      string result = task.GetScript();
      if (task.State == TaskState.Failed)
      {
        throw new AggregateException(string.Format("Failed to execute {0} task. \n{1}", task.Name, result));
      }

      string path = Path.Combine(arguments.Tasker.GlobalParams.First(p=>p.Name== "FilesRoot").Value , string.Format("generated_scripts/{0}",task.UnInstall?"Uninstall":"Install"));
      Directory.CreateDirectory(path);
      
        using (StreamWriter writer = new StreamWriter(Path.Combine(path, string.Format("{0}.ps1", task.Name))))
        {
          writer.Write(task.GetScript());
        }
      
    }
  }
}

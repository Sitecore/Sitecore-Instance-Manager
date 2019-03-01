using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class RunSitecoreTasksProcessor : ProcessorHive
  {
    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      List<Processor> processors = new List<Processor>();
      foreach (SitecoreTask task in arguments.Tasker.Tasks)
      {
        if (!task.ShouldRun)
        {
          continue;
        }

        Processor proc=null;
        if (arguments.ScriptsOnly)
        {
          proc = new GenerateScriptProcessor(task.Name);
        }
        else
        {
          proc = new RunSitecoreTaskProcessor(task.Name);
        }
        proc.Title = task.Name;
        processors.Add(proc);
      }

      return processors;
    }
  }
}

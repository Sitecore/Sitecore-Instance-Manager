using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class GenerateUnInstallParameters : Processor
  {
    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      if (arguments.Tasker.UnInstall)
      {
        this.Skip();
        return;
      }
      string filesPath = Path.Combine(ApplicationManager.UnInstallParamsFolder,arguments.Tasker.GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      Directory.CreateDirectory(filesPath);
      using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, "globals.json")))
      {
        wr.Write(arguments.Tasker.GetSerializedGlobalParams());
      }
      foreach (SitecoreTask task in arguments.Tasker.Tasks.Where(t => t.ShouldRun))
      {
        string parameters = task.GetSerializedParameters();
        using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, task.Name + ".json")))
        {
          wr.Write(parameters);
        }
      }
    }
  }
}

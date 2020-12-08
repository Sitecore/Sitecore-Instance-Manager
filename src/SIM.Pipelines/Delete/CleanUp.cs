using JetBrains.Annotations;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIM.Sitecore9Installer;

namespace SIM.Pipelines.Delete
{
  public class CleanUp : Processor
  {
    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = args as Install9Args;
      Assert.ArgumentNotNull(arguments, nameof(arguments));
      if (arguments.ScriptsOnly)
      {
        this.Skip();
        return;
      }

      Directory.Delete(arguments.Tasker.UnInstallParamsPath, true);
      InstallParam param = arguments.Tasker.GlobalParams.FirstOrDefault(p => p.Name == "DeployRoot");
      if (param!=null&&!Directory.GetFileSystemEntries(param.Value).Any())
      {
        Directory.Delete(param.Value, true);
      }
    }
  }
}

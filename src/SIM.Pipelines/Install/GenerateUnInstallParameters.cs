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
      arguments.UnInstallDataPath= arguments.Tasker.SaveUninstallData(ApplicationManager.UnInstallParamsFolder);
    }
  }
}

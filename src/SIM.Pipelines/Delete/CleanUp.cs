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
using System.Threading;
using Sitecore.Diagnostics.Logging;

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
      if (param!=null)
      {
        int retrisNumber = 3;
        for (int i=0;i<= retrisNumber; i++)
        {
          if (Directory.Exists(param.Value))
          {
            try
            {
              Directory.Delete(param.Value, true);
            }
            catch(System.IO.IOException ex)
            {
              Log.Warn($"Can't remove directory: {param.Value}. {ex.Message}");
            }
            if (Directory.Exists(param.Value))
            {
              if (retrisNumber==i)
              {
                throw new Exception($"Can't remove directory: {param.Value}");
              }
              Thread.Sleep(10000);
            }
            else
            {
              break;
            }
          }
        }
      }
    }
  }
}

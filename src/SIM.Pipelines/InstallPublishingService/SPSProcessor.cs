using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public abstract class SPSProcessor<T> : Processor where T : ProcessorArgs
  {
    protected static bool AbortPipeline = false;

    protected override void Process([NotNull] ProcessorArgs args)
    {
      T processorArgs = args as T;

      if (AbortPipeline)
      {
        Log.Warn($"The {this.GetType().Name} processor was skipped.");
        this.Skip();
        return;
      }

      try
      {
        this.ProcessCore(processorArgs);
      }
      catch (Exception ex)
      {
        AbortPipeline = true;
        Log.Error($"Publishing Service Pipeline was aborted.  The remaining steps were skipped.");
        throw ex;
      }
    }

    protected abstract void ProcessCore([NotNull] T args);

    //Kind of a hack so I can resure some of the uninstall sps processors in the RemoveExistingSPSProcessor class
    internal void DoProcess([NotNull] T args)
    {
      this.Process(args);
    }
  }
}

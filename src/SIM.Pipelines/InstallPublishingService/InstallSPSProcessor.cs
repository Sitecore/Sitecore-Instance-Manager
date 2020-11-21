using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public abstract class InstallSPSProcessor : Processor
  {
    protected static bool AbortPipeline = false;

    protected override void Process([NotNull] ProcessorArgs args)
    {
      InstallSPSProcessorArgs processorArgs = args as InstallSPSProcessorArgs;

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
        Log.Error($"installpublishingservice Pipeline was aborted.  The remaining steps were skipped.");
        throw ex;
      }
    }

    protected abstract void ProcessCore(InstallSPSProcessorArgs args);
  }
}

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
  public abstract class InstallPublishingServiceProcessor  : Processor
  {
    protected static bool AbortPipeline = false;

    protected override void Process([NotNull] ProcessorArgs args)
    {
      InstallPublishingServiceProcessorArgs processorArgs = args as InstallPublishingServiceProcessorArgs;

      if (processorArgs == null)
      {
        throw new Exception("Invalid ProcessorArgs passed to InstallPublishingServiceProcessor");
      }

      if (AbortPipeline)
      {
        Log.Warn($"{this} processor was skipped.");
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

    protected abstract void ProcessCore(InstallPublishingServiceProcessorArgs args);
  }
}

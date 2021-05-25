using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.PublishingService
{
  public abstract class SPSProcessor<T> : Processor where T : ProcessorArgs
  {
    protected override void Process([NotNull] ProcessorArgs args)
    { 
      this.ProcessCore((T)args);
    }

    protected abstract void ProcessCore([NotNull] T args);
  }
}

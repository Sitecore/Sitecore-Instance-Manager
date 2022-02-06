using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;

namespace SIM.Pipelines.MultipleDeletion
{
  public abstract class MultipleDeletion9Processor : Processor
  {
    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((MultipleDeletion9Args)args);
    }

    protected abstract void Process([NotNull] MultipleDeletion9Args args);
  }
}
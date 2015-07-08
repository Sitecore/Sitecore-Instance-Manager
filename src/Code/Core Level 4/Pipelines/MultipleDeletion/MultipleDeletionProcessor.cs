#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.MultipleDeletion
{
  public abstract class MultipleDeletionProcessor : Processor
  {
    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Process((MultipleDeletionArgs)args);
    }

    protected abstract void Process([NotNull] MultipleDeletionArgs args);
  }
}
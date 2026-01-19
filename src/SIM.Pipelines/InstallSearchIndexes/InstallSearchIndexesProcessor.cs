using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public abstract class InstallSearchIndexesProcessor : Processor
  {
    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return EvaluateStepsCount((InstallSearchIndexesArgs)args);
    }

    public override sealed bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return IsRequireProcessing((InstallSearchIndexesArgs)args);
    }

    protected virtual long EvaluateStepsCount([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((InstallSearchIndexesArgs)args);
    }

    protected abstract void Process([NotNull] InstallSearchIndexesArgs args);
  }
}

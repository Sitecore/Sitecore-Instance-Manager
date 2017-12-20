namespace SIM.Pipelines.Import
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public abstract class ImportProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      return EvaluateStepsCount((ImportArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return IsRequireProcessing((ImportArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount(ImportArgs args)
    {
      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] ImportArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((ImportArgs)args);
    }

    protected abstract void Process([NotNull] ImportArgs args);

    #endregion

    #endregion
  }
}
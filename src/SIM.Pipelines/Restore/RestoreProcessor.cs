namespace SIM.Pipelines.Restore
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public abstract class RestoreProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return this.EvaluateStepsCount((RestoreArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return this.IsRequireProcessing((RestoreArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      this.Process((RestoreArgs)args);
    }

    protected abstract void Process([NotNull] RestoreArgs args);

    #endregion

    #endregion
  }
}
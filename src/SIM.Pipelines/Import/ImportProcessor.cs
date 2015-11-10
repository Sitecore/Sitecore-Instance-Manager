namespace SIM.Pipelines.Import
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class ImportProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      return this.EvaluateStepsCount((ImportArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((ImportArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount(ImportArgs args)
    {
      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] ImportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((ImportArgs)args);
    }

    protected abstract void Process([NotNull] ImportArgs args);

    #endregion

    #endregion
  }
}
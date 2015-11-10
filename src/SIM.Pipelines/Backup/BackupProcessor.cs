namespace SIM.Pipelines.Backup
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class BackupProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      return this.EvaluateStepsCount((BackupArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((BackupArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount(BackupArgs args)
    {
      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((BackupArgs)args);
    }

    protected abstract void Process([NotNull] BackupArgs args);

    #endregion

    #endregion
  }
}
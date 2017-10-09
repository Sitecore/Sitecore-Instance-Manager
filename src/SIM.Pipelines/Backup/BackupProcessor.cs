namespace SIM.Pipelines.Backup
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public abstract class BackupProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      return EvaluateStepsCount((BackupArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return IsRequireProcessing((BackupArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount(BackupArgs args)
    {
      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((BackupArgs)args);
    }

    protected abstract void Process([NotNull] BackupArgs args);

    #endregion

    #endregion
  }
}
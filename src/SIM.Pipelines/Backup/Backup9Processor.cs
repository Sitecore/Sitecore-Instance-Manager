namespace SIM.Pipelines.Backup
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public abstract class Backup9Processor : Processor
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

      return IsRequireProcessing((Backup9Args)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount(Backup9Args args)
    {
      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] Backup9Args args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((Backup9Args)args);
    }

    protected abstract void Process([NotNull] Backup9Args args);

    #endregion

    #endregion
  }
}
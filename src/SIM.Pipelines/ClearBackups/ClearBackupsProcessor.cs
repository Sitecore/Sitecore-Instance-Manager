namespace SIM.Pipelines.ClearBackups
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Pipelines.ClearBackups;
  using SIM.Pipelines.Restore;

  #region

  #endregion

  public abstract class ClearBackupsProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return EvaluateStepsCount((RestoreArgs)args);
    }

    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return IsRequireProcessing((RestoreArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount([NotNull] ClearBackupsArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] ClearBackupsArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }


    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Process((ClearBackupsArgs)args);
    }

    protected abstract void Process([NotNull] ClearBackupsArgs args);
    #endregion

    #endregion
  }
}
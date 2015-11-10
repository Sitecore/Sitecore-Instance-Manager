namespace SIM.Pipelines.Reinstall
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public abstract class ReinstallProcessor : Processor
  {
    #region Public Methods

    public override sealed bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((ReinstallArgs)args);
    }

    #endregion

    #region Methods

    protected virtual bool IsRequireProcessing([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((ReinstallArgs)args);
    }

    protected abstract void Process([NotNull] ReinstallArgs args);

    #endregion
  }
}
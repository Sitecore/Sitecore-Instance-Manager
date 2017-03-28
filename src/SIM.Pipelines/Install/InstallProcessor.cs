namespace SIM.Pipelines.Install
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public abstract class InstallProcessor : Processor
  {
    #region Public Methods

    public override sealed bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return this.IsRequireProcessing((InstallArgs)args);
    }

    #endregion

    #region Methods

    protected virtual bool IsRequireProcessing([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      this.Process((InstallArgs)args);
    }

    protected abstract void Process([NotNull] InstallArgs args);

    #endregion
  }
}
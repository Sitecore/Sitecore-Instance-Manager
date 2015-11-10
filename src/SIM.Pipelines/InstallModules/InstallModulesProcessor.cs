namespace SIM.Pipelines.InstallModules
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public abstract class InstallModulesProcessor : Processor
  {
    #region Methods

    #region Public methods

    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.EvaluateStepsCount((InstallModulesArgs)args);
    }

    public override sealed bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((InstallModulesArgs)args);
    }

    #endregion

    #region Protected methods

    protected virtual long EvaluateStepsCount([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1;
    }

    protected virtual bool IsRequireProcessing([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((InstallModulesArgs)args);
    }

    protected abstract void Process([NotNull] InstallModulesArgs args);

    #endregion

    #endregion
  }
}
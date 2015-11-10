namespace SIM.Pipelines.Reinstall
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : ReinstallProcessor
  {
    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return InstallHelper.GetStepsCount(((ReinstallArgs)args).PackagePath);
    }

    #endregion

    #region Methods

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      InstallHelper.ExtractFile(args.PackagePath, args.WebRootPath, args.DatabasesFolderPath, args.DataFolderPath, this.Controller);
    }

    #endregion
  }
}
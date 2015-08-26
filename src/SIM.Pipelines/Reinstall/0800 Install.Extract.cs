namespace SIM.Pipelines.Reinstall
{
  using System;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : ReinstallProcessor
  {
    #region Constants

    private const int K = 40;

    #endregion

    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1; // K;
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
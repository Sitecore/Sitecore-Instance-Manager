namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateWebConfig : ReinstallProcessor
  {
    #region Constants

    private const string DataFolder = "dataFolder";

    #endregion

    #region Methods

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootPath, args.WebRootPath, args.DataFolderPath, args.ServerSideRedirect, args.IncreaseExecutionTimeout);
    }

    #endregion
  }
}
namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

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
      Assert.ArgumentNotNull(args, nameof(args));

      UpdateWebConfigHelper.Process(args.RootPath, args.WebRootPath, args.DataFolderPath, args.ServerSideRedirect, args.IncreaseExecutionTimeout, args.Product);
    }

    #endregion
  }
}
namespace SIM.Pipelines.Install
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateWebConfig : InstallProcessor
  {
    #region Methods

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootFolderPath, args.WebRootPath, args.DataFolderPath, args.ServerSideRedirect, args.IncreaseExecutionTimeout);
    }

    #endregion
  }
}
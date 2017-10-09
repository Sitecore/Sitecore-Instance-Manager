namespace SIM.Pipelines.Install
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateWebConfig : InstallProcessor
  {
    #region Methods

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      UpdateWebConfigHelper.Process(args.RootFolderPath, args.WebRootPath, args.DataFolderPath, args.ServerSideRedirect, args.IncreaseExecutionTimeout);
    }

    #endregion
  }
}
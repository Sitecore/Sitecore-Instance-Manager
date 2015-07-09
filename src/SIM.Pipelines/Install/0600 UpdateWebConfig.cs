namespace SIM.Pipelines.Install
{
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateWebConfig : InstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootFolderPath, args.WebRootPath, args.DataFolderPath);
    }

    #endregion
  }
}
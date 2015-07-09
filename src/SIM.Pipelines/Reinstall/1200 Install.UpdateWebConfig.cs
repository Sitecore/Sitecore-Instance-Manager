namespace SIM.Pipelines.Reinstall
{
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateWebConfig : ReinstallProcessor
  {
    #region Constants

    private const string DataFolder = "dataFolder";

    #endregion

    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootPath, args.WebRootPath, args.DataFolderPath);
    }

    #endregion
  }
}
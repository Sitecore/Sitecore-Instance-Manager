namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CheckPackageIntegrity : ReinstallProcessor
  {
    #region Methods

    [UsedImplicitly]
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}
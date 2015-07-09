namespace SIM.Pipelines.Install
{
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CheckPackageIntegrity : InstallProcessor
  {
    #region Methods

    [UsedImplicitly]
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}
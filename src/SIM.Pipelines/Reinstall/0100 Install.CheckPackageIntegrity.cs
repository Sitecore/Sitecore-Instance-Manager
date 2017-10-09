namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CheckPackageIntegrity : ReinstallProcessor
  {
    #region Methods

    [UsedImplicitly]
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      FileSystem.FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}
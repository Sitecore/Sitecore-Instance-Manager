namespace SIM.Pipelines.Install
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CheckPackageIntegrity : InstallProcessor
  {
    #region Methods

    [UsedImplicitly]
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      FileSystem.FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}
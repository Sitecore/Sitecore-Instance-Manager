namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteWebsiteFolder : ReinstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.FileSystem.Local.Directory.DeleteIfExists(args.WebRootPath);
    }

    #endregion
  }
}
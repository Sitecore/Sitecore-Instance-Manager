namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteRootFolder : ReinstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      string path = args.RootPath;
      if (!string.IsNullOrEmpty(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(path);

        FileSystem.FileSystem.Local.Directory.CreateDirectory(path);
      }
    }

    #endregion
  }
}
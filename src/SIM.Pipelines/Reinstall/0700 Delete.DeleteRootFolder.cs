namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteRootFolder : ReinstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var path = args.RootPath;
      if (!string.IsNullOrEmpty(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(path);

        FileSystem.FileSystem.Local.Directory.CreateDirectory(path);
      }
    }

    #endregion
  }
}
namespace SIM.Pipelines.Delete
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteRootFolder : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      string path = args.RootPath;
      if (!string.IsNullOrEmpty(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(path);
      }
    }

    #endregion
  }
}
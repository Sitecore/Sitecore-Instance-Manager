namespace SIM.Pipelines.Delete
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDataFolder : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      string path = args.InstanceDataFolderPath;
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(path);
    }

    #endregion
  }
}
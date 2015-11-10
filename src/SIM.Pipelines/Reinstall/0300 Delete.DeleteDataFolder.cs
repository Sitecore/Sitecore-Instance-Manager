namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDataFolder : ReinstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      string path = args.DataFolderPath;
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(path);
    }

    #endregion
  }
}
namespace SIM.Pipelines.Reinstall
{
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class MoveData : ReinstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      DirectoryInfo uniqueFolder = new DirectoryInfo(args.TempFolder);
      DirectoryInfo[] subdirs = uniqueFolder.GetDirectories();
      Assert.IsTrue(subdirs.Length == 1, "Something wrong with extracted files");
      DirectoryInfo extracted = subdirs[0];
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "website", args.WebRootPath);
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "data", args.DataFolderPath);
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "databases", args.DatabasesFolderPath);
    }

    #endregion
  }
}
namespace SIM.Pipelines.Install
{
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class MoveData : InstallProcessor
  {
    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      DirectoryInfo uniqueFolder = new DirectoryInfo(args.UniqueTempFolder);
      DirectoryInfo[] subdirs = uniqueFolder.GetDirectories();
      int length = subdirs.Length;
      Assert.IsTrue(length == 1, "Something wrong with extracted files - in the root of the " + args.PackagePath + " package there are {0} folders instead of expected 1".FormatWith(length));
      DirectoryInfo extracted = subdirs[0];
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "website", args.WebRootPath);
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "data", args.DataFolderPath);
      FileSystem.FileSystem.Local.Directory.MoveChild(extracted, "databases", args.DatabasesFolderPath);
    }

    #endregion
  }
}
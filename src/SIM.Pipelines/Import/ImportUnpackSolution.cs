namespace SIM.Pipelines.Import
{
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  internal class ImportUnpackSolution : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      // Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
      // Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
      // args.temporaryPathToUnpack = Path.GetTempPath();
      string webRootName = args.virtualDirectoryPhysicalPath.Split('\\')[args.virtualDirectoryPhysicalPath.Split('\\').Length - 1];
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.rootPath, "Data");
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.rootPath, webRootName);
    }

    #endregion
  }
}
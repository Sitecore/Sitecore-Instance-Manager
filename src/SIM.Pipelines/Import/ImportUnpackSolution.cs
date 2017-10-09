namespace SIM.Pipelines.Import
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  internal class ImportUnpackSolution : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      // Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
      // Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
      // args.temporaryPathToUnpack = Path.GetTempPath();
      var webRootName = args._VirtualDirectoryPhysicalPath.Split('\\')[args._VirtualDirectoryPhysicalPath.Split('\\').Length - 1];
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args._RootPath, "Data");
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args._RootPath, webRootName);
    }

    #endregion
  }
}
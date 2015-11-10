namespace SIM.Pipelines.Import
{
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ImportRestoreMongoDatabases : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      var folder = FileSystem.FileSystem.Local.Directory.Ensure(args.temporaryPathToUnpack.PathCombine("MongoDatabases"));
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.temporaryPathToUnpack, "MongoDatabases");
      foreach (var directoryPath in FileSystem.FileSystem.Local.Directory.GetDirectories(folder))
      {
        MongoHelper.Restore(directoryPath);
      }
    }

    #endregion
  }
}
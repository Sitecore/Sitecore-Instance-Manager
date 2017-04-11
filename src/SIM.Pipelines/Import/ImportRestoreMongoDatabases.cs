namespace SIM.Pipelines.Import
{
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class ImportRestoreMongoDatabases : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      var folder = FileSystem.FileSystem.Local.Directory.Ensure(args._TemporaryPathToUnpack.PathCombine("MongoDatabases"));
      FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args._TemporaryPathToUnpack, "MongoDatabases");
      foreach (var directoryPath in FileSystem.FileSystem.Local.Directory.GetDirectories(folder))
      {
        MongoHelper.Restore(directoryPath);
      }
    }

    #endregion
  }
}
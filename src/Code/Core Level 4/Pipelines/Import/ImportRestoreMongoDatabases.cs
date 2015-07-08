namespace SIM.Pipelines.Import
{
  using SIM.Base;

  [UsedImplicitly]
  public class ImportRestoreMongoDatabases : ImportProcessor
  {
    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process(ImportArgs args)
    {
      var folder = FileSystem.Local.Directory.Ensure(args.temporaryPathToUnpack.PathCombine("MongoDatabases"));
      FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.temporaryPathToUnpack, "MongoDatabases");
      foreach (var directoryPath in FileSystem.Local.Directory.GetDirectories(folder))
      {
        MongoHelper.Restore(directoryPath);
      }
    }
  }
}
namespace SIM.Adapters.MongoDb
{
  using System.IO;
  using JetBrains.Annotations;

  public class MongoDbDatabaseImport
  {
    [NotNull]
    public string DirectoryPath { get; }

    [NotNull]
    public string OriginalName { get; }

    [NotNull]
    public string FinalName { get; set; }

    public MongoDbDatabaseImport(string directoryPath)
    {
      DirectoryPath = directoryPath;
      OriginalName = Path.GetFileName(directoryPath);
      FinalName = OriginalName;
    }
  }
}
namespace SIM.Pipelines.Import
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Adapters.MongoDb;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class ImportRestoreMongoDatabases : ImportProcessor
  {
    #region Private members

    private ImportArgs Args;

    #endregion

    #region Private consts

    private const int DEFAULT_POSTFIX = -1;
    private const int MAXIMUM_DATABASE_POSTFIX_RETRY = 100;

    #endregion

    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      Args = args;

      EnsureEmptyExtractedBackupFolder();

      ExtractDatabases();
      if (!Args.ExtractedMongoDatabases.Any())
      {
        return;
      }

      SetDatabasesFinalName();

      foreach (var database in Args.ExtractedMongoDatabases)
      {
        MongoHelper.Restore(database.DirectoryPath, database.FinalName);
      }
    }

    #endregion

    #region Private methods

    private void EnsureEmptyExtractedBackupFolder()
    {
      string folderForExtractedBackups = Args._TemporaryPathToUnpack.PathCombine("MongoDatabases");
      FileSystem.FileSystem.Local.File.Delete(folderForExtractedBackups);
      FileSystem.FileSystem.Local.Directory.Ensure(folderForExtractedBackups);
    }

    private void ExtractDatabases()
    {
      Args.ExtractedMongoDatabases = new List<MongoDbDatabaseImport>();

      var folderWithExtractedBackups = FileSystem.FileSystem.Local.Zip.ZipUnpackFolder(Args.PathToExportedInstance, Args._TemporaryPathToUnpack, "MongoDatabases");

      foreach (string directoryPath in FileSystem.FileSystem.Local.Directory.GetDirectories(folderWithExtractedBackups))
      {
        if (FileSystem.FileSystem.Local.Directory.GetFiles(directoryPath).Any())
        {
          Args.ExtractedMongoDatabases.Add(new MongoDbDatabaseImport(directoryPath));
        }
      }
    }

    private void SetDatabasesFinalName()
    {
      int postfix = GetDatabasePostfix();

      if (postfix != DEFAULT_POSTFIX)
      {
        foreach (var database in Args.ExtractedMongoDatabases)
        {
          database.FinalName = GetDatabaseName(database.OriginalName, postfix);
        }
      }
    }

    private int GetDatabasePostfix()
    {
      int postfix = DEFAULT_POSTFIX;

      while (postfix < MAXIMUM_DATABASE_POSTFIX_RETRY)
      {
        if (postfix == MAXIMUM_DATABASE_POSTFIX_RETRY - 1)
        {
          throw new Exception("(Import: SIM.Pipelines.Import.ImportRestoreMongoDatabases) GetDatabasePostfix method timeout. ");
        }

        bool existingDatabaseFound = false;

        foreach (var database in Args.ExtractedMongoDatabases)
        {
          if (MongoDbManager.Instance.DatabaseExists(GetDatabaseName(database.OriginalName, postfix)))
          {
            existingDatabaseFound = true;
            break;
          }
        }

        if (existingDatabaseFound)
        {
          postfix++;
        }
        else
        {
          break;
        }
      }

      return postfix;
    }

    public string GetDatabaseName(string originalName, int postfix)
    {
      if (postfix != DEFAULT_POSTFIX)
      {
        return $"{originalName}_{postfix}";
      }

      return originalName;
    }

    #endregion
  }
}
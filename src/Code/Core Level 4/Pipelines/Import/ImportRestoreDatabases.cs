using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;
using System.IO;
using SIM.Adapters.SqlServer;
using System.Data.SqlClient;



namespace SIM.Pipelines.Import
{
  using SIM.Pipelines.Restore;

  [UsedImplicitly]
  class ImportRestoreDatabases : ImportProcessor
  {

    #region Fields    
    #endregion
    //
    protected override void Process(ImportArgs args)
    {
      //SqlServerManager.Instance.BackupInfo b = new SqlServerManager.Instance.BackupInfo();
      //SqlServerManager.Instance.GetDatabasesNameFromBackup(
      RestoreDatabases(args);//DEBUG
    }

    List<string> ExtractDatabases(ImportArgs args)
    {
      List<string> result = new List<string>();
      //
      string folderWithExtractedBackups = FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.rootPath.PathCombine("Databases"), "Databases");
      //
      foreach (string file in FileSystem.Local.Directory.GetFiles(folderWithExtractedBackups, "*.bak"))
      {
        result.Add(file);
      }
      //
      return result;
    }
    //
    void RestoreDatabases(ImportArgs args)
    {
      if (FileSystem.Local.Directory.Exists(args.temporaryPathToUnpack.PathCombine("Databases")))
      {
        foreach (string file in FileSystem.Local.Directory.GetFiles(args.temporaryPathToUnpack.PathCombine("Databases"))) FileSystem.Local.File.Delete(file);
      }
      List<string> backupsPaths = ExtractDatabases(args);
      if (backupsPaths.Count == 0) return;
      var backupInfo = new SqlServerManager.BackupInfo();
      GetPostfixForDatabases(backupsPaths, args.connectionString, ref args.databaseNameAppend);

      foreach (string backup in backupsPaths)
      {
        backupInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(args.connectionString, backup);
        string dbName = backupInfo.dbOriginalName;
        //dbName = GetDatabaseName(dbName, args.connectionString, ref args.databaseNameAppend);
        dbName = GetDatabaseName(dbName, ref args.databaseNameAppend);
        //dbName = dbName + GetDBNameAppend(dbName, args.connectionString, 0);
        SqlServerManager.Instance.RestoreDatabase(dbName,
                                          args.connectionString,
                                          backup,
                                          FileSystem.Local.Directory.GetParent(args.virtualDirectoryPhysicalPath).FullName.PathCombine("Databases"),
                                          backupInfo);
      }

    }

    //
    //bool DatabaseExist(List<string> dbNames, SqlConnectionStringBuilder connectionString, string postfix)
    //  {
    //    foreach(string dbName in dbNames)
    //    {
    //      if(SqlServerManager.Instance.DatabaseExists(dbName+postfix, connectionString)) return true; 
    //    }
    //    return false;
    //  }
    //
    //string GetDatabasesPostfix(List<string> backupsPaths, SqlConnectionStringBuilder connectionString, int counter)
    //{
    //  List<string> dbNames = new List<string>();
    //  var backupInfo = new SqlServerManager.BackupInfo();
    //  foreach (string backup in backupsPaths)
    //  {
    //    backupInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(connectionString, backup);
    //    dbNames.Add(backupInfo.GetDatabaseName());
    //  }
    //  //
    //  if (!DatabaseExist(dbNames, connectionString, "")) return "";
    //  while (DatabaseExist(dbNames, connectionString, "_" + counter.ToString()))
    //  {
    //    counter++;
    //  }
    //  return "_" + counter.ToString();
    //}
    //
    public string GetDatabaseName(string oldName, SqlConnectionStringBuilder connectionString, ref int postfix)
    {
        int postFix = postfix;
        string newName = oldName;
        while (true)
        {
            if (SqlServerManager.Instance.DatabaseExists(newName, connectionString))
            {
                newName = oldName;
                postFix++;
                newName += "_" + postFix.ToString(); 
            }
            else
            {
                if(postFix !=-1)
                    postfix = postFix;

                return newName;
            }
        }
        throw new Exception("Can't get a new DB name. (SIM.Pipelines.Import.ImportRestoreDatabases)");
    }
    //
    public string GetDatabaseName(string oldName, ref int postfix)
    {
        if (postfix != -1)
            return oldName + "_" + postfix.ToString();
        else return oldName;
    }
    //
    public void GetPostfixForDatabases(IEnumerable<string> dbBackupsPaths, SqlConnectionStringBuilder connectionString, ref int postfix)
    {
        List<string> dbNames = new List<string>();
        //
        foreach (string backupPath in dbBackupsPaths)
        {
            dbNames.Add(SqlServerManager.Instance.GetDatabasesNameFromBackup(connectionString, backupPath).dbOriginalName);
        }

        //
        int i=0;
        int counter = 0;
        while (counter<100)//todo while true
        {
            if (counter == 99) throw new Exception("(Import: SIM.Pipelines.Import.ImportRestoreDatabases) GetPostfixForDatabases method timeout. ");
            if (SqlServerManager.Instance.DatabaseExists(dbNames[i], connectionString) && postfix == -1)
            {
                postfix++;
            }
            else if(SqlServerManager.Instance.DatabaseExists(dbNames[i] + "_" + postfix.ToString(), connectionString))
            {
                postfix++;
            }
            else if (i == dbNames.Count - 1)
            {
                if (CheckDatabases(dbNames, connectionString, ref postfix))
                    return;
                else
                {
                    i = 0;
                }
                    
            }
            else
            {
                i++;
            }
            counter++;
        }
    }
    //
    public bool CheckDatabases(IEnumerable<string> dbNames, SqlConnectionStringBuilder connectionString, ref int postfix)
    {
        foreach (string dbName in dbNames)
        {
            if (SqlServerManager.Instance.DatabaseExists(dbName + "_" + postfix.ToString(), connectionString))
            {
                return false;
            }
        }
        return true;
    }

  }
}

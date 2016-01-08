namespace SIM.Pipelines
{
  #region Usings

  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #endregion

  public static class AttachDatabasesHelper
  {
    // total number of steps in entire install/reinstall wizard without this one is around 5000, so we can assume attaching databases takes 5% so 250
    public const int StepsCount = 250;

    #region Public methods

    public static void AttachDatabase(string name, string databasesFolderPath, ConnectionString connectionString, SqlConnectionStringBuilder defaultConnectionString, IPipelineController controller)
    {
      SetConnectionStringNode(name, defaultConnectionString, connectionString);

      string databaseName = connectionString.GenerateDatabaseName(name);

      string databasePath =
        DatabaseFilenameHook(Path.Combine(databasesFolderPath, connectionString.DefaultFileName), 
          connectionString.Name.Replace("yafnet", "forum"), databasesFolderPath);

      if (!IsRemoteSqlServer())
      {
        if (!FileSystem.FileSystem.Local.File.Exists(databasePath))
        {
          var file = Path.GetFileName(databasePath);
          if (file.EqualsIgnoreCase("sitecore.reporting.mdf"))
          {
            databasePath = Path.Combine(Path.GetDirectoryName(databasePath), "Sitecore.Analytics.mdf");
          }
        }

        FileSystem.FileSystem.Local.File.AssertExists(databasePath, databasePath + " file doesn't exist");
      }

      if (SqlServerManager.Instance.DatabaseExists(databaseName, defaultConnectionString))
      {
        databaseName = ResolveConflict(defaultConnectionString, connectionString, databasePath, databaseName, controller);
      }

      if (databaseName != null)
      {
        SqlServerManager.Instance.AttachDatabase(databaseName, databasePath, defaultConnectionString);
      }
    }

    public static void AttachDatabase(ConnectionString connectionString, SqlConnectionStringBuilder defaultConnectionString, string name, string databasesFolderPath, string instanceName, IPipelineController controller)
    {
      if (connectionString.IsMongoConnectionString)
      {
        connectionString.Value = AttachDatabasesHelper.GetMongoConnectionString(connectionString.Name, instanceName);
        connectionString.SaveChanges();
        return;
      }

      if (connectionString.IsSqlConnectionString)
      {
        try
        {
          AttachDatabasesHelper.AttachDatabase(name, databasesFolderPath, connectionString, defaultConnectionString, controller);
        }
        catch (Exception ex)
        {
          if (connectionString.Name == "reporting.secondary")
          {
            throw;
          }

          Log.Warn(ex, "Attaching reporting.secondary database failed. Skipping...");
        }
      }
    }

    [NotNull]
    public static string DatabaseFilenameHook([NotNull] string databasePath, [NotNull] string databaseName, [NotNull] string databasesFolderPath)
    {
      Assert.ArgumentNotNull(databasePath, "databasePath");
      Assert.ArgumentNotNull(databaseName, "databaseName");

      if (IsRemoteSqlServer())
      {
        databasePath = GetLocalDatabasePathOnRemoteServer(databasePath);
      }

      if (!FileSystem.FileSystem.Local.File.Exists(databasePath) && !IsRemoteSqlServer())
      {
        string[] files = FileSystem.FileSystem.Local.Directory.GetFiles(databasesFolderPath, "*" + databaseName + ".mdf");
        string file = files.SingleOrDefault();
        if (!string.IsNullOrEmpty(file) && FileSystem.FileSystem.Local.File.Exists(file))
        {
          return file;
        }
      }

      return databasePath;
    }

    public static string GetRemoteDatabaseFilePath(string databaseFilePath)
    {
      Assert.ArgumentNotNullOrEmpty(databaseFilePath, "databasesFilePath");
      Assert.IsTrue(Path.IsPathRooted(databaseFilePath), "The path is not rooted (" + databaseFilePath + ")");

      var remoteDatabaseServerName = SqlServerManager.Settings.CoreSqlServerRemoteServerName.Value;
      var virtualPath = FileSystem.FileSystem.Local.Directory.GetVirtualPath(databaseFilePath);
      var value = @"\\{0}\{1}\{2}".FormatWith(remoteDatabaseServerName, ReplaceRemoteServerVariable(databaseFilePath).TrimEnd('\\'), virtualPath);
      return value;
    }

    public static bool IsRemoteSqlServer()
    {
      return !string.IsNullOrEmpty(SqlServerManager.Settings.CoreSqlServerRemoteServerName.Value.Trim());
    }

    public static void MoveDatabases(string databasesFolderPath)
    {
      if (AttachDatabasesHelper.IsRemoteSqlServer())
      {
        foreach (var filePath in FileSystem.FileSystem.Local.Directory.GetFiles(databasesFolderPath))
        {
          var remoteDatabaseFilePath = AttachDatabasesHelper.GetRemoteDatabaseFilePath(filePath);
          FileSystem.FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(remoteDatabaseFilePath));
          FileSystem.FileSystem.Local.File.Move(filePath, remoteDatabaseFilePath);
        }

        FileSystem.FileSystem.Local.Directory.DeleteIfExists(databasesFolderPath);
      }
    }

    public static string ResolveConflict(SqlConnectionStringBuilder defaultConnectionString, ConnectionString connectionString, string databasePath, string databaseName, IPipelineController controller)
    {
      string existingDatabasePath = SqlServerManager.Instance.GetDatabaseFileName(databaseName, defaultConnectionString);

      if (string.IsNullOrEmpty(existingDatabasePath))
      {
        var m = "The database with the same '{0}' name is already exists in the SQL Server metabase but points to non-existing file. ".FormatWith(databaseName);
        if (controller.Confirm(m + "Would you like to delete it?"))
        {
          SqlServerManager.Instance.DeleteDatabase(databaseName, defaultConnectionString);
          return databaseName;
        }

        throw new Exception(m);
      }

      if (existingDatabasePath.EqualsIgnoreCase(databasePath))
      {
        return null;
      }

      // todo: replce this with shiny message box
      string delete = "Delete the '{0}' database".FormatWith(databaseName);
      const string AnotherName = "Use another database name";
      const string Cancel = "Terminate current action";
      string[] options = new[]
      {
        delete, AnotherName, Cancel
      };
      string m2 = "The database with '{0}' name already exists".FormatWith(databaseName);
      string result = controller.Select(m2, options);
      switch (result)
      {
        case Cancel:
          throw new Exception(m2);
        case AnotherName:
          databaseName = ResolveConflictByUnsedName(defaultConnectionString, connectionString, databaseName);
          break;
        default:
          SqlServerManager.Instance.DeleteDatabase(databaseName, defaultConnectionString);
          break;
      }

      return databaseName;
    }

    #endregion

    #region Private methods

    private static string GetLocalDatabasePathOnRemoteServer(string databasePath)
    {
      var virtualPath = FileSystem.FileSystem.Local.Directory.GetVirtualPath(databasePath);
      databasePath = ReplaceRemoteServerVariable(databasePath).TrimStart("\\").Replace("$\\", ":\\").EnsureEnd("\\") + virtualPath;
      return databasePath;
    }

    [NotNull]
    private static string GetMongoConnectionString([NotNull] string connectionStringName, [NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(connectionStringName, "connectionStringName");
      Assert.ArgumentNotNull(instanceName, "instanceName");

      var mongoDbName = instanceName + "_" + connectionStringName;
      var invalidChars = new[]
      {
        '\0', ' ', '.', '$', '/', '\\'
      };
      foreach (var @char in invalidChars)
      {
        mongoDbName = mongoDbName.Replace(@char, "_"); // #SIM-128 Fixed
      }

      var value = Settings.AppMongoConnectionString.Value.TrimEnd('/') + @"/" + mongoDbName;
      return value;
    }

    [NotNull]
    private static string GetUnusedDatabaseName([NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(defaultConnectionString, "defaultConnectionString");
      Assert.ArgumentNotNull(databaseName, "databaseName");

      const int K = 100;
      for (int i = 1; i <= K; i++)
      {
        if (!SqlServerManager.Instance.DatabaseExists(databaseName + '_' + i, defaultConnectionString))
        {
          return databaseName + "_" + i;
        }
      }

      throw new InvalidOperationException("Something weird happen... Do you really have '{0}' file?".FormatWith(databaseName + "_" + K));
    }

    private static string ReplaceRemoteServerVariable(string databasePath)
    {
      var remoteFolderName = SqlServerManager.Settings.CoreSqlServerRemoteFolderName.Value;
      return remoteFolderName.Replace("{DRIVE_LETTER}", Path.GetPathRoot(databasePath)[0]);
    }

    [NotNull]
    private static string ResolveConflictByUnsedName([NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] ConnectionString connectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(defaultConnectionString, "defaultConnectionString");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(databaseName, "databaseName");

      databaseName = GetUnusedDatabaseName(defaultConnectionString, databaseName);
      connectionString.RealName = databaseName;
      connectionString.SaveChanges();
      return databaseName;
    }

    private static void SetConnectionStringNode([NotNull] string name, [NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] ConnectionString connectionString)
    {
      Assert.ArgumentNotNull(defaultConnectionString, "defaultConnectionString");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(defaultConnectionString.ConnectionString)
      {
        InitialCatalog = connectionString.GenerateDatabaseName(name)
      };
      connectionString.Value = builder.ToString();
      connectionString.SaveChanges();
    }

    #endregion
  }
}
namespace SIM.Pipelines
{
  #region Usings

  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Base;
  using SIM.Pipelines.Install;

  #endregion

  /// <summary>
  /// The attach databases helper.
  /// </summary>
  public static class AttachDatabasesHelper
  {
    #region Public methods

    /// <summary>
    /// The resolve conflict.
    /// </summary>
    /// <param name="defaultConnectionString">
    /// The default connection string.
    /// </param>
    /// <param name="connectionString">
    /// The connection string.
    /// </param>
    /// <param name="databasePath">
    /// The database path.
    /// </param>
    /// <param name="databaseName">
    /// The database name.
    /// </param>
    /// <param name="controller">
    /// The controller.
    /// </param>
    /// <returns>
    /// The resolve conflict.
    /// </returns>
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
      string[] options = new[] { delete, AnotherName, Cancel };
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

    public static void AttachDatabase(string name, string databasesFolderPath, ConnectionString connectionString, SqlConnectionStringBuilder defaultConnectionString, IPipelineController controller)
    {
      SetConnectionStringNode(name, defaultConnectionString, connectionString);

      string databaseName = connectionString.GenerateDatabaseName(name);

      string databasePath =
        DatabaseFilenameHook(Path.Combine(databasesFolderPath, connectionString.DefaultFileName),
                             connectionString.Name.Replace("yafnet", "forum"), databasesFolderPath);

      if (!IsRemoteSqlServer())
      {
        if (!FileSystem.Local.File.Exists(databasePath))
        {
          var file = Path.GetFileName(databasePath);
          if (file.EqualsIgnoreCase("sitecore.reporting.mdf"))
          {
            databasePath = Path.Combine(Path.GetDirectoryName(databasePath), "Sitecore.Analytics.mdf");
          }
        }

        FileSystem.Local.File.AssertExists(databasePath, databasePath + " file doesn't exist");
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

    #endregion

    #region Private methods

    [NotNull]
    public static string DatabaseFilenameHook([NotNull] string databasePath, [NotNull] string databaseName, [NotNull] string databasesFolderPath)
    {
      Assert.ArgumentNotNull(databasePath, "databasePath");
      Assert.ArgumentNotNull(databaseName, "databaseName");

      if (IsRemoteSqlServer())
      {
        databasePath = GetLocalDatabasePathOnRemoteServer(databasePath);
      }

      if (!FileSystem.Local.File.Exists(databasePath) && !IsRemoteSqlServer())
      {
        string[] files = FileSystem.Local.Directory.GetFiles(databasesFolderPath, "*" + databaseName + ".mdf");
        string file = files.SingleOrDefault();
        if (!string.IsNullOrEmpty(file) && FileSystem.Local.File.Exists(file))
        {
          return file;
        }
      }

      return databasePath;
    }

    private static string GetLocalDatabasePathOnRemoteServer(string databasePath)
    {
      var virtualPath = FileSystem.Local.Directory.GetVirtualPath(databasePath);
      databasePath = ReplaceRemoteServerVariable(databasePath).TrimStart("\\").Replace("$\\", ":\\").EnsureEnd("\\") + virtualPath;
      return databasePath;
    }

    private static string ReplaceRemoteServerVariable(string databasePath)
    {
      var remoteFolderName = SqlServerManager.Settings.CoreSqlServerRemoteFolderName.Value;
      return remoteFolderName.Replace("{DRIVE_LETTER}", Path.GetPathRoot(databasePath)[0]);
    }

    public static bool IsRemoteSqlServer()
    {
      return !string.IsNullOrEmpty(SqlServerManager.Settings.CoreSqlServerRemoteServerName.Value.Trim());
    }

    /// <summary>
    /// The set connection string node.
    /// </summary>
    /// <param name="name"> The name. </param>
    /// <param name="defaultConnectionString">
    /// The default connection string. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    private static void SetConnectionStringNode([NotNull] string name, [NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] ConnectionString connectionString)
    {
      Assert.ArgumentNotNull(defaultConnectionString, "defaultConnectionString");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(defaultConnectionString.ConnectionString) { InitialCatalog = connectionString.GenerateDatabaseName(name), IntegratedSecurity = false };
      connectionString.Value = builder.ToString();
      connectionString.SaveChanges();
    }

    /// <summary>
    /// The get unused database name.
    /// </summary>
    /// <param name="defaultConnectionString">
    /// The default Connection String. 
    /// </param>
    /// <param name="databaseName">
    /// The database Name. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Something weird happen... Do you really have '{0}' file?
    /// </exception>
    /// <returns>
    /// The get unused database name. 
    /// </returns>
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

    /// <summary>
    /// The resolve conflict by unsed name.
    /// </summary>
    /// <param name="defaultConnectionString">
    /// The default connection string. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <returns>
    /// The resolve conflict by unsed name. 
    /// </returns>
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

    #endregion

    public static string GetRemoteDatabaseFilePath(string databaseFilePath)
    {
      Assert.ArgumentNotNullOrEmpty(databaseFilePath, "databasesFilePath");
      Assert.IsTrue(Path.IsPathRooted(databaseFilePath), "The path is not rooted (" + databaseFilePath + ")");

      var remoteDatabaseServerName = SqlServerManager.Settings.CoreSqlServerRemoteServerName.Value;
      var virtualPath = FileSystem.Local.Directory.GetVirtualPath(databaseFilePath);
      var value = @"\\{0}\{1}\{2}".FormatWith(remoteDatabaseServerName, ReplaceRemoteServerVariable(databaseFilePath).TrimEnd('\\'), virtualPath);
      return value;
    }

    public static void MoveDatabases(string databasesFolderPath)
    {
      if (AttachDatabasesHelper.IsRemoteSqlServer())
      {
        foreach (var filePath in FileSystem.Local.Directory.GetFiles(databasesFolderPath))
        {
          var remoteDatabaseFilePath = AttachDatabasesHelper.GetRemoteDatabaseFilePath(filePath);
          FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(remoteDatabaseFilePath));
          FileSystem.Local.File.Move(filePath, remoteDatabaseFilePath);
        }

        FileSystem.Local.Directory.DeleteIfExists(databasesFolderPath);
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

          Log.Warn("Attaching reporting.secondary database failed. Skipping...", typeof(AttachDatabasesHelper), ex);
        }
      }      
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
  }
}
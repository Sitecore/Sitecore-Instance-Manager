namespace SIM.Adapters.SqlServer
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using System.Net;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public class SqlServerManager
  {
    #region Constants

    public const string BackupExtension = ".bak";

    #endregion

    #region Fields

    public static readonly SqlServerManager Instance = new SqlServerManager();
    public static readonly int SqlServerConnectionTimeout;

    #endregion

    #region Constructors

    static SqlServerManager()
    {
      SqlServerConnectionTimeout = Settings.CoreSqlServerConnectionTimeout.Value;
    }

    #endregion

    #region Public Methods

    #region Public methods

    public virtual void AttachDatabase([NotNull] string name, [NotNull] string path, [NotNull] SqlConnectionStringBuilder connectionString, bool attachLog = true)
    {
      Assert.ArgumentNotNullOrEmpty(name, "name");
      Assert.ArgumentNotNullOrEmpty(path, "path");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      Log.Info("Attaching the '{0}' database with '{1}' filename", name, path);

      var sqlServerAccountName = this.GetSqlServerAccountName(connectionString);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(Path.GetDirectoryName(path), sqlServerAccountName);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(path, sqlServerAccountName);
      var ldf = Path.ChangeExtension(path, ".ldf");
      if (FileSystem.FileSystem.Local.File.Exists(ldf))
      {
        FileSystem.FileSystem.Local.Security.EnsurePermissions(ldf, sqlServerAccountName);
      }

      using (SqlConnection sqlConnection = this.OpenConnection(connectionString))
      {
        string command = string.Format("create database [{0}] on (filename = N'{1}'){2} for attach", name, path, attachLog && FileSystem.FileSystem.Local.File.Exists(ldf) ? ", (filename = N'" + ldf + "')" : string.Empty);
        this.Execute(sqlConnection, command);
      }
    }

    public virtual void BackupDatabase(SqlConnectionStringBuilder connectionString, string databaseName, string pathToBackup)
    {
      Log.Info("Backuping the '{0}' database", databaseName);

      using (SqlConnection sqlConnection = this.OpenConnection(connectionString))
      {
        string command = string.Format(@"
            BACKUP DATABASE [{0}]
            TO DISK = '{1}'          
            ", new object[]
        {
          databaseName, pathToBackup
        });
        this.Execute(sqlConnection, command);
      }
    }

    [NotNull]
    public virtual SqlConnectionStringBuilder ChangeDatabaseName([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(databaseName, "databaseName");

      connectionString = new SqlConnectionStringBuilder(connectionString.ConnectionString)
      {
        InitialCatalog = databaseName
      };

      return connectionString;
    }

    public virtual void CloseConnectionsToDatabase(string dbName, SqlConnection sqlConnection)
    {
      Log.Info("Closing connection to the '{0}' database", dbName);
      string command = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbName);
      this.Execute(sqlConnection, command);
    }

    public virtual bool DatabaseExists([NotNull] string name, [NotNull] SqlConnection sqlConnection)
    {
      Assert.ArgumentNotNull(name, "name");
      Assert.ArgumentNotNull(sqlConnection, "sqlConnection");

      string command = string.Format("select [name] from [master].[sys].[databases] where [name] = N'{0}'", name);
      using (SqlCommand sqlCmd = new SqlCommand(command, sqlConnection))
      {
        using (SqlDataReader reader = sqlCmd.ExecuteReader())
        {
          if (reader.HasRows)
          {
            return true;
          }
        }
      }

      return false;
    }

    public virtual bool DatabaseExists([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      using (SqlConnection connection = this.OpenConnection(connectionString))
      {
        return DatabaseExists(databaseName, connection);
      }
    }

    public virtual void DeleteDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      using (SqlConnection connection = this.OpenConnection(connectionString))
      {
        DeleteDatabase(databaseName, connection);
      }
    }

    public virtual void DetachDatabase(string realName, SqlConnectionStringBuilder connectionString)
    {
      Log.Info("Detaching the '{0}' database", realName);
      using (SqlConnection sqlConnection = this.OpenConnection(connectionString))
      {
        this.CloseConnectionsToDatabase(realName, sqlConnection);
        string command = string.Format("EXEC master.dbo.sp_detach_db @dbname = N'{0}', @skipchecks = 'false'", realName);
        this.Execute(sqlConnection, command);
      }
    }

    public virtual void DetectDatabases(string rootPath, SqlConnectionStringBuilder connectionString, Action<string> action)
    {
      Assert.ArgumentNotNullOrEmpty(rootPath, "rootPath");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(action, "action");
      rootPath = rootPath.TrimEnd('\\') + '\\';
      foreach (var name in this.GetDatabasesNames(connectionString))
      {
        var file = this.GetDatabaseFileName(name, connectionString);
        if (string.IsNullOrEmpty(file) || !FileSystem.FileSystem.Local.File.Exists(file))
        {
          continue;
        }

        string directory = Path.GetDirectoryName(file);
        if (string.IsNullOrEmpty(directory) || !FileSystem.FileSystem.Local.Directory.Exists(directory))
        {
          continue;
        }

        string directoryName = directory.TrimEnd('\\') + '\\';
        if (directoryName.ContainsIgnoreCase(rootPath))
        {
          action(name);
        }
      }
    }

    public virtual void Execute([NotNull] SqlConnection sqlConnection, [NotNull] string command, int? executionTimeout = null)
    {
      Assert.ArgumentNotNull(sqlConnection, "sqlConnection");
      Assert.ArgumentNotNull(command, "command");

      Log.Info("SQL query is executed: {0}", command);

      using (SqlCommand sqlCmd = new SqlCommand(command, sqlConnection)
      {
        CommandTimeout = executionTimeout ?? Settings.CoreSqlServerExecutionTimeout.Value
      })
      {
        sqlCmd.ExecuteNonQuery();
      }
    }

    public virtual void Execute([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string command, int? executionTimeout = null)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(command, "command");

      using (var connection = OpenConnection(connectionString, false))
      {
        this.Execute(connection, command);
      }
    }

    [NotNull]
    public virtual string GenerateDatabaseRealName([NotNull] string instanceName, [NotNull] string connectionStringName, [CanBeNull] string productName = null, [CanBeNull] string pattern = null)
    {
      Assert.ArgumentNotNull(instanceName, "instanceName");
      Assert.ArgumentNotNull(connectionStringName, "connectionStringName");

      return pattern.EmptyToNull() ??
             Settings.CoreSqlServerDatabaseNamePattern.Value
               .Replace("{InstanceName}", instanceName)
               .Replace("{DatabaseRole}", connectionStringName)
               .Replace("miniForum", "Forum")
               .Replace("{ProductName}", productName.EmptyToNull() ?? "Sitecore");
    }

    [CanBeNull]
    public virtual string GetDatabaseFileName([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      try
      {
        using (SqlConnection connection = this.OpenConnection(connectionString))
        {
          return GetDatabaseFileName(databaseName, connection);
        }
      }
      catch (SqlException ex)
      {
        if (ex.Message.ContainsIgnoreCase("Unable to open the physical file"))
        {
          return null;
        }

        throw;
      }
    }

    [NotNull]
    public virtual IEnumerable<string> GetDatabaseFolders([NotNull] IEnumerable<Database> databases)
    {
      Assert.ArgumentNotNull(databases, "databases");

      // ReSharper disable AssignNullToNotNullAttribute
      return databases.Where(d => !string.IsNullOrEmpty(d.FileName) && FileSystem.FileSystem.Local.File.Exists(d.FileName)).Select(d => Path.GetDirectoryName(d.FileName)).Distinct();

      // ReSharper restore AssignNullToNotNullAttribute
    }

    [NotNull]
    public virtual string GetDatabaseNameFromFile([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string pathToMdf)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      string res = string.Empty;

      using (SqlConnection conn = this.OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand(@"dbcc checkprimaryfile (N'" + pathToMdf + @"' , 2)", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          if ((string)reader["property"] == "Database name" && !((string)reader["value"]).IsNullOrEmpty())
          {
            res = (string)reader["value"];
          }
        }
      }

      return res;
    }

    [NotNull]
    public virtual BackupInfo GetDatabasesNameFromBackup([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string pathToBak)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      BackupInfo res = new BackupInfo(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

      using (SqlConnection conn = this.OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand("RESTORE FILELISTONLY FROM DISK='" + pathToBak + "'", conn);

        // SqlCommand command = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          string logicName = (string)reader["LogicalName"];
          string physName = (string)reader["PhysicalName"];
          if (physName.ToLower().Contains(".mdf"))
          {
            res.logicalNameMdf = logicName;
            res.physicalNameMdf = physName;
          }
          else
          {
            res.logicalNameLdf = logicName;
            res.physicalNameLdf = physName;
          }
        }

        reader.Close();
        SqlCommand command2 = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
        reader = command2.ExecuteReader();
        string dbName = string.Empty;
        while (reader.Read())
        {
          dbName = (string)reader["DatabaseName"];
        }

        res.dbOriginalName = dbName;
      }

      return res;
    }

    [NotNull]
    public virtual List<string> GetDatabasesNames([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      List<string> res = new List<string>();

      using (SqlConnection conn = this.OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand("USE master; SELECT [Name] FROM sys.databases", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          string item = (string)reader["name"];
          if (item.EqualsIgnoreCase("master") || item.EqualsIgnoreCase("model") || item.EqualsIgnoreCase("msdb") || item.EqualsIgnoreCase("tempdb"))
          {
            continue;
          }

          res.Add(item);
        }
      }

      return res;
    }

    [NotNull]
    public virtual List<string> GetDatabasesNames([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string searchPattern)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      List<string> res = new List<string>();

      using (SqlConnection conn = this.OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand(@"USE master; SELECT [Name] FROM sys.databases WHERE [Name] LIKE '%" + searchPattern + @"%'", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          string item = (string)reader["name"];
          if (item.EqualsIgnoreCase("master") || item.EqualsIgnoreCase("model") || item.EqualsIgnoreCase("msdb") || item.EqualsIgnoreCase("tempdb") || !item.ContainsIgnoreCase(searchPattern))
          {
            continue;
          }

          res.Add(item);
        }
      }

      return res;
    }

    public virtual DataTable GetResultOfQueryExecution([NotNull] SqlConnectionStringBuilder connectionString, string sqlQuery)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNullOrEmpty(sqlQuery, "sqlQuery");

      DataTable dataTable;

      using (SqlConnection conn = this.OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand(sqlQuery, conn);
        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

        dataTable = new DataTable();
        sqlDataAdapter.Fill(dataTable);
      }

      return dataTable;
    }

    public virtual string GetSqlServerAccountName(SqlConnectionStringBuilder connectionString)
    {
      try
      {
        using (var connection = this.OpenConnection(connectionString))
        {
          var command = new SqlCommand
          {
            Connection = connection, 
            CommandText = @"DECLARE @ServiceaccountName varchar(250)
                  EXECUTE master.dbo.xp_instance_regread
                  N'HKEY_LOCAL_MACHINE',
                  N'SYSTEM\CurrentControlSet\Services\MSSQLSERVER',
                  N'ObjectName',
                  @ServiceAccountName OUTPUT,
                  N'no_output'
                  SELECT @ServiceaccountName"
          };

          var adapter = new SqlDataAdapter
          {
            SelectCommand = command
          };

          var dataTable = new DataTable();
          adapter.Fill(dataTable);

          return dataTable.Rows[0][0] as string;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "GetSqlServerAccountName");
        throw new InvalidOperationException("Cannot retrieve SQL Server Account Name");
      }
    }

    public virtual bool IsConnectionStringValid([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      try
      {
        var resultConnectionString = this.GetManagementConnectionString(connectionString, 1);
        using (this.OpenConnection(resultConnectionString))
        {
        }

        return true;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during checking connection string {0}", connectionString.ToString());

        return false;
      }
    }

    public virtual bool IsMongoConnectionString(string connectionString)
    {
      return connectionString.StartsWith(@"mongodb://");
    }

    public virtual bool IsSqlConnectionString([CanBeNull] string connectionString)
    {
      try
      {
        new SqlConnectionStringBuilder(connectionString);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    [NotNull]
    public virtual SqlConnection OpenConnection([NotNull] SqlConnectionStringBuilder connectionString, bool isManagement = true)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      SqlConnectionStringBuilder managementConnectionString = isManagement ? this.GetManagementConnectionString(connectionString) : connectionString;
      SqlConnection connection = new SqlConnection(managementConnectionString.ConnectionString);
      connection.Open();
      return connection;
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = this.OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        this.CloseConnectionsToDatabase(databaseName, connection);

        Log.Info("Restoring database: {0}", databaseName);
        string restoreCommand = "RESTORE DATABASE [" + databaseName + "] FROM  DISK = N'" + backupFileName + "' WITH REPLACE, RECOVERY --force restore over specified database";
        this.Execute(connection, restoreCommand);
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(pathTo, "pathTo");
      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = this.OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        string mdfName = string.Empty;
        string ldfName = string.Empty;
        if (backupInfo.logicalNameMdf.IsNullOrEmpty())
        {
          mdfName = databaseName + ".Data";
        }
        else
        {
          mdfName = backupInfo.logicalNameMdf;
        }

        if (backupInfo.logicalNameLdf.IsNullOrEmpty())
        {
          ldfName = databaseName + ".Log";
        }
        else
        {
          ldfName = backupInfo.logicalNameLdf;
        }

        Log.Info("Restoring database: {0}", databaseName);
        string command = string.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{0}.mdf',
                        MOVE '{4}' TO'{2}\{0}_log.ldf'", 
          new[]
          {
            databaseName /*backupInfo.dbOriginalName*/, backupFileName, pathTo, mdfName, ldfName
          });
        this.Execute(connection, command, 60 * 60 * 2); // 2 hours
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] string databaseFileName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(pathTo, "pathTo");
      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = this.OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        string mdfName = string.Empty;
        string ldfName = string.Empty;
        if (backupInfo.logicalNameMdf.IsNullOrEmpty())
        {
          mdfName = databaseName + ".Data";
        }
        else
        {
          mdfName = backupInfo.logicalNameMdf;
        }

        if (backupInfo.logicalNameLdf.IsNullOrEmpty())
        {
          ldfName = databaseName + ".Log";
        }
        else
        {
          ldfName = backupInfo.logicalNameLdf;
        }

        Log.Info("Restoring database: {0}", databaseName);
        string command = string.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{5}.mdf',
                        MOVE '{4}' TO'{2}\{5}_log.ldf'", 
          new[]
          {
            databaseName, backupFileName, pathTo, mdfName, ldfName, databaseFileName
          });
        this.Execute(connection, command);
      }
    }

    public virtual bool TestSqlServer(string rootPath, string connectionString)
    {
      var createDatabase = string.Format("CREATE DATABASE TestDatabase ON PRIMARY (NAME = TestDatabase_Data, FILENAME = '{0}\\TestDatabase.mdf', SIZE = 20MB, MAXSIZE = 100MB, FILEGROWTH = 10%) " +
                                         "LOG ON (NAME = TestDatabase_Log, FILENAME = '{0}\\TestDatabase.ldf', SIZE = 10MB, MAXSIZE = 50MB, FILEGROWTH = 10%)", rootPath);

      try
      {
        using (var connection = new SqlConnection(connectionString))
        {
          connection.Open();

          var command = new SqlCommand
          {
            Connection = connection, 
            CommandText = createDatabase
          };

          command.ExecuteNonQuery();
          const string dropDatabase = "DROP DATABASE TestDatabase";
          command = new SqlCommand(dropDatabase, connection);


          command.ExecuteNonQuery();
          return true;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot create a test database");

        return false;
      }
    }

    [UsedImplicitly]
    public virtual void ValidateConnectionString([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      try
      {
        var resultConnectionString = this.GetManagementConnectionString(connectionString, 1);
        using (this.OpenConnection(resultConnectionString, false))
        {
        }
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("The provided SQL Server connection string isn't valid. Reason:\n\n" + ex.Message);
      }
    }

    #endregion

    public struct BackupInfo
    {
      #region Fields

      public string dbOriginalName;
      public string logicalNameLdf;
      public string logicalNameMdf;
      public string physicalNameLdf;
      public string physicalNameMdf;

      #endregion


      #region Constructors

      public BackupInfo(string logicalNameOfMdf, string physicalNameOfMdf, string logicalNameOfLdf, string physicalNameOfLdf, string databaseOriginalName)
      {
        this.logicalNameMdf = logicalNameOfMdf;
        this.physicalNameMdf = physicalNameOfMdf;
        this.logicalNameLdf = logicalNameOfLdf;
        this.physicalNameLdf = physicalNameOfLdf;
        this.dbOriginalName = databaseOriginalName;
      }

      #endregion


      #region Public methods

      public string GetDatabaseName()
      {
        if (!this.logicalNameMdf.IsNullOrEmpty())
        {
          return this.logicalNameMdf.Replace(".Data", string.Empty);
        }

        if (!this.logicalNameLdf.IsNullOrEmpty())
        {
          return this.logicalNameLdf.Replace(".Log", string.Empty);
        }

        return string.Empty;
      }

      #endregion

    }

    #endregion

    #region Methods

    #region Public methods

    [NotNull]
    public virtual SqlConnectionStringBuilder GetManagementConnectionString([NotNull] SqlConnectionStringBuilder connectionString, int? timeout = null)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");
      connectionString = new SqlConnectionStringBuilder(connectionString.ConnectionString)
      {
        InitialCatalog = "master", 
        ConnectTimeout = timeout != null ? (int)timeout : SqlServerConnectionTimeout
      };

      return connectionString;
    }

    #endregion

    #region Protected methods

    protected virtual void DeleteDatabase([NotNull] string databaseName, [NotNull] SqlConnection connection)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connection, "connection");

      Log.Info("Deleting database: '{0}'", databaseName);

      const string dropDatabase = "DROP DATABASE [{0}]";

      try
      {
        this.CloseConnectionsToDatabase(databaseName, connection);
        this.Execute(connection, dropDatabase.FormatWith(databaseName));
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during database '{0}' deleting attempt. Retrying...");
        var command = "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{0}'";
        this.Execute(connection, command.FormatWith(databaseName));
      }
    }

    [CanBeNull]
    protected virtual string GetDatabaseFileName([NotNull] string databaseName, [NotNull] SqlConnection connection)
    {
      string command = @"exec sp_helpdb [{0}]".FormatWith(databaseName);
      if (DatabaseExists(databaseName, connection))
      {
        using (SqlCommand sqlCommand = new SqlCommand(command, connection))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
          sqlCommand.ExecuteNonQuery();
          using (DataSet dataSet = new DataSet())
          {
            sqlDataAdapter.Fill(dataSet);
            try
            {
              return dataSet.Tables[1].Rows[0]["filename"].ToString().EmptyToNull();
            }
            catch (Exception ex)
            {
              Log.Warn(ex, "Cannot get database file name for {0}", databaseName);
            }
          }
        }
      }

      return null;
    }

    #endregion

    #endregion

    #region Public methods

    public virtual string NormalizeServerName(string serverName)
    {
      string name = serverName.ToLowerInvariant();
      switch (name)
      {
        case ".":
          return ".";
        case "localhost":
          return ".";
        default:
          string hostName = Dns.GetHostName();
          if (name.EqualsIgnoreCase(hostName) || name.EqualsIgnoreCase(Environment.MachineName) || Dns.GetHostAddresses(hostName).Any(address => address.ToString().Equals(name)))
          {
            return ".";
          }

          return serverName;
      }
    }

    #endregion

    #region Nested type: Settings

    public static class Settings
    {
      #region Fields

      public static readonly AdvancedProperty<int> CoreSqlServerConnectionTimeout = AdvancedSettings.Create("Core/SqlServer/ConnectionTimeout", 1);

      public static readonly AdvancedProperty<string> CoreSqlServerDatabaseNamePattern = AdvancedSettings.Create("Core/SqlServer/DatabaseNamePattern", "{InstanceName}{ProductName}_{DatabaseRole}");
      public static readonly AdvancedProperty<int> CoreSqlServerExecutionTimeout = AdvancedSettings.Create("Core/SqlServer/ExecutionTimeout", 180);

      #endregion
    }

    #endregion
  }
}
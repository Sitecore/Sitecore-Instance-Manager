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
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public class SqlServerManager
  {
    #region Constants

    public const string BackupExtension = ".bak";

    #endregion

    #region Fields

    public static SqlServerManager Instance { get; } = new SqlServerManager();
    public static int SqlServerConnectionTimeout { get; }

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
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      Log.Info($"Attaching the '{name}' database with '{path}' filename");

      var sqlServerAccountName = GetSqlServerAccountName(connectionString);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(Path.GetDirectoryName(path), sqlServerAccountName);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(path, sqlServerAccountName);
      var ldf = Path.ChangeExtension(path, ".ldf");
      if (FileSystem.FileSystem.Local.File.Exists(ldf))
      {
        FileSystem.FileSystem.Local.Security.EnsurePermissions(ldf, sqlServerAccountName);
      }

      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        var command = $"create database [{name}] on (filename = N'{path}'){(attachLog && FileSystem.FileSystem.Local.File.Exists(ldf) ? ", (filename = N'" + ldf + "')" : string.Empty)} for attach";
        Execute(sqlConnection, command);
      }
    }

    public virtual void BackupDatabase(SqlConnectionStringBuilder connectionString, string databaseName, string pathToBackup)
    {
      Log.Info($"Backuping the '{databaseName}' database");

      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        var command = string.Format(@"
            BACKUP DATABASE [{0}]
            TO DISK = '{1}'          
            ", new object[]
        {
          databaseName, pathToBackup
        });
        Execute(sqlConnection, command);
      }
    }

    [NotNull]
    public virtual SqlConnectionStringBuilder ChangeDatabaseName([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      connectionString = new SqlConnectionStringBuilder(connectionString.ConnectionString)
      {
        InitialCatalog = databaseName
      };

      return connectionString;
    }

    public virtual void CloseConnectionsToDatabase(string dbName, SqlConnection sqlConnection)
    {
      Log.Info($"Closing connection to the '{dbName}' database");
      var command = $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
      Execute(sqlConnection, command);
    }

    public virtual bool DatabaseExists([NotNull] string name, [NotNull] SqlConnection sqlConnection)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(sqlConnection, nameof(sqlConnection));

      var command = $"select [name] from [master].[sys].[databases] where [name] = N'{name}'";
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
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        return DatabaseExists(databaseName, connection);
      }
    }

    public virtual void DeleteDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        DeleteDatabase(databaseName, connection);
      }
    }

    public virtual void DetachDatabase(string realName, SqlConnectionStringBuilder connectionString)
    {
      Log.Info($"Detaching the '{realName}' database");
      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        CloseConnectionsToDatabase(realName, sqlConnection);
        var command = $"EXEC master.dbo.sp_detach_db @dbname = N'{realName}', @skipchecks = 'false'";
        Execute(sqlConnection, command);
      }
    }

    public virtual void DetectDatabases(string rootPath, SqlConnectionStringBuilder connectionString, Action<string> action)
    {
      Assert.ArgumentNotNullOrEmpty(rootPath, nameof(rootPath));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(action, nameof(action));
      rootPath = rootPath.TrimEnd('\\') + '\\';
      foreach (var name in GetDatabasesNames(connectionString))
      {
        var file = GetDatabaseFileName(name, connectionString);
        if (string.IsNullOrEmpty(file) || !FileSystem.FileSystem.Local.File.Exists(file))
        {
          continue;
        }

        var directory = Path.GetDirectoryName(file);
        if (string.IsNullOrEmpty(directory) || !FileSystem.FileSystem.Local.Directory.Exists(directory))
        {
          continue;
        }

        var directoryName = directory.TrimEnd('\\') + '\\';
        if (directoryName.ContainsIgnoreCase(rootPath))
        {
          action(name);
        }
      }
    }

    public virtual void Execute([NotNull] SqlConnection sqlConnection, [NotNull] string command, int? executionTimeout = null)
    {
      Assert.ArgumentNotNull(sqlConnection, nameof(sqlConnection));
      Assert.ArgumentNotNull(command, nameof(command));

      Log.Info($"SQL query is executed: {command}");

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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(command, nameof(command));

      using (var connection = OpenConnection(connectionString, false))
      {
        Execute(connection, command);
      }
    }

    [NotNull]
    public virtual string GenerateDatabaseRealName([NotNull] string instanceName, [NotNull] string sqlPrefix, [NotNull] string connectionStringName, [CanBeNull] string productName = null, [CanBeNull] string pattern = null)
    {
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));
      Assert.ArgumentNotNull(sqlPrefix, nameof(sqlPrefix));
      Assert.ArgumentNotNull(connectionStringName, nameof(connectionStringName));

      return pattern.EmptyToNull() ??
             Settings.CoreSqlServerDatabaseNamePattern.Value
               .Replace("{SqlPrefix}", sqlPrefix)
               .Replace("{InstanceName}", instanceName)
               .Replace("{DatabaseRole}", connectionStringName)
               .Replace("miniForum", "Forum")
               .Replace("{ProductName}", productName.EmptyToNull() ?? "Sitecore");
    }

    [CanBeNull]
    public virtual string GetDatabaseFileName([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      try
      {
        using (SqlConnection connection = OpenConnection(connectionString))
        {
          return GetDatabaseFileName(databaseName, connection);
        }
      }
      catch (SqlException ex)
      {
        if (Extensions.ContainsIgnoreCase(ex.Message, "Unable to open the physical file"))
        {
          return null;
        }

        throw;
      }
    }

    [NotNull]
    public virtual IEnumerable<string> GetDatabaseFolders([NotNull] IEnumerable<Database> databases)
    {
      Assert.ArgumentNotNull(databases, nameof(databases));

      // ReSharper disable AssignNullToNotNullAttribute
      return databases.Where(d => !string.IsNullOrEmpty(d.FileName) && FileSystem.FileSystem.Local.File.Exists(d.FileName)).Select(d => Path.GetDirectoryName(d.FileName)).Distinct();

      // ReSharper restore AssignNullToNotNullAttribute
    }

    [NotNull]
    public virtual string GetDatabaseNameFromFile([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string pathToMdf)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      var res = string.Empty;

      using (SqlConnection conn = OpenConnection(connectionString))
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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      BackupInfo res = new BackupInfo(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

      using (SqlConnection conn = OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand("RESTORE FILELISTONLY FROM DISK='" + pathToBak + "'", conn);

        // SqlCommand command = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          var logicName = (string)reader["LogicalName"];
          var physName = (string)reader["PhysicalName"];
          if (physName.ToLower().Contains(".mdf"))
          {
            res._LogicalNameMdf = logicName;
            res._PhysicalNameMdf = physName;
          }
          else
          {
            res._LogicalNameLdf = logicName;
            res._PhysicalNameLdf = physName;
          }
        }

        reader.Close();
        SqlCommand command2 = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
        reader = command2.ExecuteReader();
        var dbName = string.Empty;
        while (reader.Read())
        {
          dbName = (string)reader["DatabaseName"];
        }

        res._DbOriginalName = dbName;
      }

      return res;
    }

    [NotNull]
    public virtual List<string> GetDatabasesNames([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      List<string> res = new List<string>();

      using (SqlConnection conn = OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand("USE master; SELECT [Name] FROM sys.databases", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          var item = (string)reader["name"];
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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      List<string> res = new List<string>();

      using (SqlConnection conn = OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand(@"USE master; SELECT [Name] FROM sys.databases WHERE [Name] LIKE '%" + searchPattern + @"%'", conn);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          var item = (string)reader["name"];
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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNullOrEmpty(sqlQuery, nameof(sqlQuery));

      DataTable dataTable;

      using (SqlConnection conn = OpenConnection(connectionString))
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
        using (var connection = OpenConnection(connectionString))
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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      try
      {
        var resultConnectionString = GetManagementConnectionString(connectionString, 1);
        using (OpenConnection(resultConnectionString))
        {
        }

        return true;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"An error occurred during checking connection string {connectionString.ToString()}");

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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      SqlConnectionStringBuilder managementConnectionString = isManagement ? GetManagementConnectionString(connectionString) : connectionString;
      SqlConnection connection = new SqlConnection(managementConnectionString.ConnectionString);
      connection.Open();
      return connection;
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, nameof(databaseName));
        Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));

        CloseConnectionsToDatabase(databaseName, connection);

        Log.Info($"Restoring database: {databaseName}");
        var restoreCommand = "RESTORE DATABASE [" + databaseName + "] FROM  DISK = N'" + backupFileName + "' WITH REPLACE, RECOVERY --force restore over specified database";
        Execute(connection, restoreCommand);
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(pathTo, nameof(pathTo));
      Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, nameof(databaseName));
        Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));

        var mdfName = string.Empty;
        var ldfName = string.Empty;
        if (backupInfo._LogicalNameMdf.IsNullOrEmpty())
        {
          mdfName = databaseName + ".Data";
        }
        else
        {
          mdfName = backupInfo._LogicalNameMdf;
        }

        if (backupInfo._LogicalNameLdf.IsNullOrEmpty())
        {
          ldfName = databaseName + ".Log";
        }
        else
        {
          ldfName = backupInfo._LogicalNameLdf;
        }

        Log.Info($"Restoring database: {databaseName}");
        var command = string.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{0}.mdf',
                        MOVE '{4}' TO'{2}\{0}_log.ldf'", 
          new[]
          {
            databaseName /*backupInfo.dbOriginalName*/, backupFileName, pathTo, mdfName, ldfName
          });
        Execute(connection, command, 60 * 60 * 2); // 2 hours
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] string databaseFileName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(pathTo, nameof(pathTo));
      Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));
      Assert.IsTrue(FileSystem.FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, nameof(databaseName));
        Assert.ArgumentNotNull(backupFileName, nameof(backupFileName));

        var mdfName = string.Empty;
        var ldfName = string.Empty;
        if (backupInfo._LogicalNameMdf.IsNullOrEmpty())
        {
          mdfName = databaseName + ".Data";
        }
        else
        {
          mdfName = backupInfo._LogicalNameMdf;
        }

        if (backupInfo._LogicalNameLdf.IsNullOrEmpty())
        {
          ldfName = databaseName + ".Log";
        }
        else
        {
          ldfName = backupInfo._LogicalNameLdf;
        }

        Log.Info($"Restoring database: {databaseName}");
        var command = string.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{5}.mdf',
                        MOVE '{4}' TO'{2}\{5}_log.ldf'", 
          new[]
          {
            databaseName, backupFileName, pathTo, mdfName, ldfName, databaseFileName
          });
        Execute(connection, command);
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
          const string DropDatabase = "DROP DATABASE TestDatabase";
          command = new SqlCommand(DropDatabase, connection);


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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      try
      {
        var resultConnectionString = GetManagementConnectionString(connectionString, 1);
        using (OpenConnection(resultConnectionString, false))
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

      public string _DbOriginalName;
      public string _LogicalNameLdf;
      public string _LogicalNameMdf;
      public string _PhysicalNameLdf;
      public string _PhysicalNameMdf;

      #endregion


      #region Constructors

      public BackupInfo(string logicalNameOfMdf, string physicalNameOfMdf, string logicalNameOfLdf, string physicalNameOfLdf, string databaseOriginalName)
      {
        _LogicalNameMdf = logicalNameOfMdf;
        _PhysicalNameMdf = physicalNameOfMdf;
        _LogicalNameLdf = logicalNameOfLdf;
        _PhysicalNameLdf = physicalNameOfLdf;
        _DbOriginalName = databaseOriginalName;
      }

      #endregion


      #region Public methods

      public string GetDatabaseName()
      {
        if (!_LogicalNameMdf.IsNullOrEmpty())
        {
          return _LogicalNameMdf.Replace(".Data", string.Empty);
        }

        if (!_LogicalNameLdf.IsNullOrEmpty())
        {
          return _LogicalNameLdf.Replace(".Log", string.Empty);
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
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
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
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connection, nameof(connection));

      Log.Info($"Deleting database: '{databaseName}'");

      const string DropDatabase = "DROP DATABASE [{0}]";

      try
      {
        CloseConnectionsToDatabase(databaseName, connection);
        Execute(connection, DropDatabase.FormatWith(databaseName));
      }
      catch (Exception ex)
      {
        Log.Warn(ex, string.Format("An error occurred during database '{0}' deleting attempt. Retrying..."));
        var command = "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{0}'";
        Execute(connection, command.FormatWith(databaseName));
      }
    }

    [CanBeNull]
    protected virtual string GetDatabaseFileName([NotNull] string databaseName, [NotNull] SqlConnection connection)
    {
      var command = @"exec sp_helpdb [{0}]".FormatWith(databaseName);
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
              return Extensions.EmptyToNull(dataSet.Tables[1].Rows[0]["filename"].ToString());
            }
            catch (Exception ex)
            {
              Log.Warn(ex, $"Cannot get database file name for {databaseName}");
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
      var name = serverName.ToLowerInvariant();
      switch (name)
      {
        case ".":
          return ".";
        case "localhost":
          return ".";
        default:
          var hostName = Dns.GetHostName();
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

      public static readonly AdvancedProperty<string> CoreSqlServerDatabaseNamePattern = AdvancedSettings.Create("Core/SqlServer/DatabaseNamePattern", "{SqlPrefix}_{DatabaseRole}");
      public static readonly AdvancedProperty<int> CoreSqlServerExecutionTimeout = AdvancedSettings.Create("Core/SqlServer/ExecutionTimeout", 180);

      #endregion
    }

    #endregion
  }
}
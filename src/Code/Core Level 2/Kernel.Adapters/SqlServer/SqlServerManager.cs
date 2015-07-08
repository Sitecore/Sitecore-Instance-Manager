#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using SIM.Base;
using System.Collections;

#endregion

namespace SIM.Adapters.SqlServer
{
  #region



  #endregion

  /// <summary>
  ///   The sql server.
  /// </summary>
  public class SqlServerManager
  {
    #region Constants

    /// <summary>
    ///   The backup extension.
    /// </summary>
    public const string BackupExtension = ".bak";

    #endregion

    #region Fields

    /// <summary>
    ///   The sql server connection timeout.
    /// </summary>
    public static readonly int SqlServerConnectionTimeout;

    public static readonly SqlServerManager Instance = new SqlServerManager();

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes static members of the <see cref="SqlServerManager" /> class.
    /// </summary>
    static SqlServerManager()
    {
      SqlServerConnectionTimeout = Settings.CoreSqlServerConnectionTimeout.Value;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The attach database.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    public virtual void AttachDatabase([NotNull] string name, [NotNull] string path, [NotNull] SqlConnectionStringBuilder connectionString, bool attachLog = true)
    {
      Assert.ArgumentNotNullOrEmpty(name, "name");
      Assert.ArgumentNotNullOrEmpty(path, "path");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      Log.Info("Attaching the '{0}' database with '{1}' filename".FormatWith(name, path), typeof(SqlServerManager));

      var sqlServerAccountName = GetSqlServerAccountName(connectionString);
      FileSystem.Local.Security.EnsurePermissions(Path.GetDirectoryName(path), sqlServerAccountName);
      FileSystem.Local.Security.EnsurePermissions(path, sqlServerAccountName);
      var ldf = Path.ChangeExtension(path, ".ldf");
      if (FileSystem.Local.File.Exists(ldf))
      {
        FileSystem.Local.Security.EnsurePermissions(ldf, sqlServerAccountName);
      }

      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        string command = string.Format("create database [{0}] on (filename = N'{1}'){2} for attach", name, path, (attachLog && FileSystem.Local.File.Exists(ldf) ? ", (filename = N'" + ldf + "')" : string.Empty));
        Execute(sqlConnection, command);
      }
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
        Log.Error("GetSqlServerAccountName", this, ex);
        throw new InvalidOperationException("Cannot retrieve SQL Server Account Name", ex);
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
        Log.Error("Cannot create a test database", this, ex);

        return false;
      }
    }

    /// <summary>
    /// The change database.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <returns>
    /// The change database. 
    /// </returns>
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

    /// <summary>
    /// The database exists.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="sqlConnection">
    /// The sql connection. 
    /// </param>
    /// <returns>
    /// The database exists. 
    /// </returns>
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

    /// <summary>
    /// The database exists.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The database exists. 
    /// </returns>
    public virtual bool DatabaseExists([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        return DatabaseExists(databaseName, connection);
      }
    }

    /// <summary>
    /// The delete database.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    public virtual void DeleteDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        DeleteDatabase(databaseName, connection);
      }
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="sqlConnection">
    /// The sql connection. 
    /// </param>
    /// <param name="command">
    /// The command. 
    /// </param>
    public virtual void Execute([NotNull] SqlConnection sqlConnection, [NotNull] string command, int? executionTimeout = null)
    {
      Assert.ArgumentNotNull(sqlConnection, "sqlConnection");
      Assert.ArgumentNotNull(command, "command");

      Log.Info("SQL query is executed: {0}".FormatWith(command), typeof(SqlServerManager));

      using (SqlCommand sqlCmd = new SqlCommand(command, sqlConnection)
      {
        CommandTimeout = executionTimeout ?? Settings.CoreSqlServerExecutionTimeout.Value
      })
      {
        sqlCmd.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// The generate database real name.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name. 
    /// </param>
    /// <param name="connectionStringName">
    /// The connection string name. 
    /// </param>
    /// <param name="productName">
    /// The product name. 
    /// </param>
    /// <param name="pattern">
    /// The pattern. 
    /// </param>
    /// <returns>
    /// The generate database real name. 
    /// </returns>
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

    /// <summary>
    /// The get database file name.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The get database file name. 
    /// </returns>
    [CanBeNull]
    public virtual string GetDatabaseFileName([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      try
      {
        using (SqlConnection connection = OpenConnection(connectionString))
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

    /// <summary>
    /// The get database folders.
    /// </summary>
    /// <param name="databases">
    /// The databases. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public virtual IEnumerable<string> GetDatabaseFolders([NotNull] IEnumerable<Database> databases)
    {
      Assert.ArgumentNotNull(databases, "databases");

      // ReSharper disable AssignNullToNotNullAttribute
      return databases.Where(d => !string.IsNullOrEmpty(d.FileName) && FileSystem.Local.File.Exists(d.FileName)).Select(d => Path.GetDirectoryName(d.FileName)).Distinct();

      // ReSharper restore AssignNullToNotNullAttribute
    }

    /// <summary>
    /// The get databases names.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public virtual List<string> GetDatabasesNames([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      List<string> res = new List<string>();

      using (SqlConnection conn = OpenConnection(connectionString))
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="pathToMdf"></param>
    /// <returns></returns>
    [NotNull]
    public virtual string GetDatabaseNameFromFile([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string pathToMdf)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      string res = "";

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

    public struct BackupInfo
    {
      public string logicalNameMdf;
      public string physicalNameMdf;

      public string logicalNameLdf;
      public string physicalNameLdf;
      public string dbOriginalName;
      //
      public BackupInfo(string logicalNameOfMdf, string physicalNameOfMdf, string logicalNameOfLdf, string physicalNameOfLdf, string databaseOriginalName)
      {
        logicalNameMdf = logicalNameOfMdf;
        physicalNameMdf = physicalNameOfMdf;
        logicalNameLdf = logicalNameOfLdf;
        physicalNameLdf = physicalNameOfLdf;
        dbOriginalName = databaseOriginalName;
      }
      //
      public string GetDatabaseName()
      {
        if (!logicalNameMdf.IsNullOrEmpty())
          return logicalNameMdf.Replace(".Data", "");
        if (!logicalNameLdf.IsNullOrEmpty())
          return logicalNameLdf.Replace(".Log", "");
        return "";
      }
      //
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="pathToBak"></param>
    /// <returns></returns>
    [NotNull]
    public virtual BackupInfo GetDatabasesNameFromBackup([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string pathToBak)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      BackupInfo res = new BackupInfo("", "", "", "", "");

      using (SqlConnection conn = OpenConnection(connectionString))
      {
        SqlCommand command = new SqlCommand("RESTORE FILELISTONLY FROM DISK='" + pathToBak + "'", conn);
        //SqlCommand command = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
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
        //
        SqlCommand command2 = new SqlCommand("RESTORE HEADERONLY FROM DISK = N'" + pathToBak + "' WITH NOUNLOAD", conn);
        reader = command2.ExecuteReader();
        string dbName = "";
        while (reader.Read())
        {
          dbName = (string)reader["DatabaseName"];
        }
        res.dbOriginalName = dbName;

      }
      return res;
    }

    /// <summary>
    /// The get databases names.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="searchPattern">
    /// The search pattern.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public virtual List<string> GetDatabasesNames([NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string searchPattern)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      List<string> res = new List<string>();

      using (SqlConnection conn = OpenConnection(connectionString))
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

    /// <summary>
    /// The is connection string valid.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The is connection string valid. 
    /// </returns>
    public virtual bool IsConnectionStringValid([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

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
        Log.Warn("An error occurred during checking connection string {0}".FormatWith(connectionString.ToString()), this, ex);

        return false;
      }
    }

    /// <summary>
    /// The is sql connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The is sql connection string. 
    /// </returns>
    public virtual bool IsSqlConnectionString([CanBeNull] string connectionString)
    {
      try
      {
        new SqlConnectionStringBuilder(connectionString);
        return true;
      }
      catch (Exception )
      {
        return false;
      }
    }

    /// <summary>
    /// The is mongo connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The is mongo connection string. 
    /// </returns>
    public virtual bool IsMongoConnectionString(string connectionString)
    {
      return connectionString.StartsWith(@"mongodb://");
    }

    /// <summary>
    /// Opens connection.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="isManagement">
    /// The is management. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public virtual SqlConnection OpenConnection([NotNull] SqlConnectionStringBuilder connectionString, bool isManagement = true)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      SqlConnectionStringBuilder managementConnectionString = isManagement ? GetManagementConnectionString(connectionString) : connectionString;
      SqlConnection connection = new SqlConnection(managementConnectionString.ConnectionString);
      connection.Open();
      return connection;
    }

    /// <summary>
    /// The restore database.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="backupFileName">
    /// The backup file name. 
    /// </param>
    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        CloseConnectionsToDatabase(databaseName, connection);

        Log.Info("Restoring database: {0}".FormatWith(databaseName), typeof(FileSystem));
        string restoreCommand = "RESTORE DATABASE [" + databaseName + "] FROM  DISK = N'" + backupFileName + "' WITH REPLACE, RECOVERY --force restore over specified database";
        Execute(connection, restoreCommand);
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(pathTo, "pathTo");
      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        string mdfName = "";
        string ldfName = "";
        //
        if (backupInfo.logicalNameMdf.IsNullOrEmpty())
          mdfName = databaseName + ".Data";
        else
          mdfName = backupInfo.logicalNameMdf;

        if (backupInfo.logicalNameLdf.IsNullOrEmpty())
          ldfName = databaseName + ".Log";
        else
          ldfName = backupInfo.logicalNameLdf;
        //
        Log.Info("Restoring database: {0}".FormatWith(databaseName), typeof(FileSystem));
        string command = String.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{0}.mdf',
                        MOVE '{4}' TO'{2}\{0}_log.ldf'",
        new string[] { databaseName /*backupInfo.dbOriginalName*/, backupFileName, pathTo, mdfName, ldfName });
        Execute(connection, command, 60 * 60 * 2); // 2 hours
      }
    }

    public virtual void RestoreDatabase([NotNull] string databaseName, [NotNull]string databaseFileName, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string backupFileName, [NotNull] string pathTo, [NotNull] BackupInfo backupInfo)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(pathTo, "pathTo");
      Assert.ArgumentNotNull(backupFileName, "backupFileName");
      Assert.IsTrue(FileSystem.Local.File.Exists(backupFileName), "Backup file is missing");

      using (SqlConnection connection = OpenConnection(connectionString))
      {
        Assert.ArgumentNotNull(databaseName, "databaseName");
        Assert.ArgumentNotNull(backupFileName, "backupFileName");

        string mdfName = "";
        string ldfName = "";
        //
        if (backupInfo.logicalNameMdf.IsNullOrEmpty())
          mdfName = databaseName + ".Data";
        else
          mdfName = backupInfo.logicalNameMdf;

        if (backupInfo.logicalNameLdf.IsNullOrEmpty())
          ldfName = databaseName + ".Log";
        else
          ldfName = backupInfo.logicalNameLdf;
        //
        Log.Info("Restoring database: {0}".FormatWith(databaseName), typeof(FileSystem));
        string command = String.Format(@"
                        RESTORE DATABASE [{0}]
                        FROM DISK='{1}'
                        WITH MOVE'{3}' TO '{2}\{5}.mdf',
                        MOVE '{4}' TO'{2}\{5}_log.ldf'",
        new string[] { databaseName, backupFileName, pathTo, mdfName, ldfName, databaseFileName });
        Execute(connection, command);
      }
    }

    /// <summary>
    /// Validates the connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The provided SQL Server connection string isn't valid. Message:
    /// </exception>
    [UsedImplicitly]
    public virtual void ValidateConnectionString([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      try
      {
        var resultConnectionString = GetManagementConnectionString(connectionString, 1);
        using (OpenConnection(resultConnectionString, false))
        {
        }
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("The provided SQL Server connection string isn't valid. Reason:\n\n" + ex.Message, ex);
      }
    }

    public virtual void DetachDatabase(string realName, SqlConnectionStringBuilder connectionString)
    {
      Log.Info("Detaching the '{0}' database".FormatWith(realName), typeof(SqlServerManager));
      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        CloseConnectionsToDatabase(realName, sqlConnection);
        string command = string.Format("EXEC master.dbo.sp_detach_db @dbname = N'{0}', @skipchecks = 'false'", realName);
        Execute(sqlConnection, command);
      }
    }

    public virtual void CloseConnectionsToDatabase(string dbName, SqlConnection sqlConnection)
    {
      Log.Info("Closing connection to the '{0}' database".FormatWith(dbName), typeof(SqlServerManager));
      string command = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbName);
      Execute(sqlConnection, command);
    }

    public virtual void BackupDatabase(SqlConnectionStringBuilder connectionString, string databaseName, string pathToBackup)
    {
      Log.Info("Backuping the '{0}' database".FormatWith(databaseName), typeof(SqlServerManager));

      using (SqlConnection sqlConnection = OpenConnection(connectionString))
      {
        string command = string.Format(@"
            BACKUP DATABASE [{0}]
            TO DISK = '{1}'          
            ", new object[] { databaseName, pathToBackup });
        Execute(sqlConnection, command);
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
        if (string.IsNullOrEmpty(file) || !FileSystem.Local.File.Exists(file))
          continue;
        string directory = Path.GetDirectoryName(file);
        if (string.IsNullOrEmpty(directory) || !FileSystem.Local.Directory.Exists(directory))
          continue;
        string directoryName = directory.TrimEnd('\\') + '\\';
        if (directoryName.ContainsIgnoreCase(rootPath))
        {
          action(name);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionString">Connection string.</param>
    /// <param name="sqlQuery">SQL query to execute.</param>
    /// <returns></returns>
    public virtual DataTable GetResultOfQueryExecution([NotNull] SqlConnectionStringBuilder connectionString, string sqlQuery)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNullOrEmpty(sqlQuery, "sqlQuery");

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

    #endregion

    #region Methods

    #region Private methods

    /// <summary>
    /// The delete database.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connection">
    /// The connection. 
    /// </param>
    protected virtual void DeleteDatabase([NotNull] string databaseName, [NotNull] SqlConnection connection)
    {
      Assert.ArgumentNotNull(databaseName, "databaseName");
      Assert.ArgumentNotNull(connection, "connection");

      Log.Info("Deleting database: '{0}'".FormatWith(databaseName), typeof(SqlServerManager));

      const string dropDatabase = "DROP DATABASE [{0}]";

      try
      {
        CloseConnectionsToDatabase(databaseName, connection);
        Execute(connection, dropDatabase.FormatWith(databaseName));
      }
      catch (Exception ex)
      {
        Log.Warn("An error occurred during database '{0}' deleting attempt. Retrying...", this, ex);
        var command = "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{0}'";
        Execute(connection, command.FormatWith(databaseName));
      }
    }

    /// <summary>
    /// The get database file name.
    /// </summary>
    /// <param name="databaseName">
    /// The database name. 
    /// </param>
    /// <param name="connection">
    /// The connection. 
    /// </param>
    /// <returns>
    /// The get database file name. 
    /// </returns>
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
              Log.Warn("Cannot get database file name for {0}".FormatWith(databaseName), typeof(SqlServerManager), ex);
            }
          }
        }
      }

      return null;
    }

    #endregion

    /// <summary>
    /// The get management connection string.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="timeout">
    /// The timeout. 
    /// </param>
    /// <returns>
    /// The get management connection string. 
    /// </returns>
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

    public static class Settings
    {
      public readonly static AdvancedProperty<int> CoreSqlServerConnectionTimeout = AdvancedSettings.Create("Core/SqlServer/ConnectionTimeout", 1);

      public readonly static AdvancedProperty<int> CoreSqlServerExecutionTimeout = AdvancedSettings.Create("Core/SqlServer/ExecutionTimeout", 180);

      public readonly static AdvancedProperty<string> CoreSqlServerDatabaseNamePattern = AdvancedSettings.Create("Core/SqlServer/DatabaseNamePattern", "{InstanceName}{ProductName}_{DatabaseRole}");

      public readonly static AdvancedProperty<string> CoreSqlServerRemoteServerName = AdvancedSettings.Create("Core/SqlServer/Remote/ServerName", "");

      public readonly static AdvancedProperty<string> CoreSqlServerRemoteFolderName = AdvancedSettings.Create("Core/SqlServer/Remote/FolderName", "{DRIVE_LETTER}$\\");
    }
  }
}
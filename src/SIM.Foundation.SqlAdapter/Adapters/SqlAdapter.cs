namespace SIM.Adapters
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using JetBrains.Annotations;
  using Microsoft.SqlServer.Dac;
  using Sitecore.Diagnostics.Base;
  using SIM.Base.Services;
  using SIM.Base.FileSystem;
  using SIM.IO;

  public sealed class SqlAdapter
  {                                                                                               
    public SqlAdapter([NotNull] SqlConnectionString connectionString)
    {
      ConnectionString = connectionString;
    }

    [NotNull]
    private SqlConnectionString ConnectionString { get; }
                                                                                                               
    /// <exception cref="SqlAdapterException" />
    public void DeployDatabase([NotNull] string databaseName, [NotNull] IFile bacpac)
    {
      Assert.ArgumentCondition(string.Equals(bacpac.Extension, ".dacpac", StringComparison.OrdinalIgnoreCase), nameof(bacpac), $"The actual file extension is '{bacpac}' which does not match expected '.dacpac'.");
      Assert.ArgumentCondition(bacpac.Exists, nameof(bacpac), $"The file does not exist by the '{bacpac}' path.");
      
      try
      {
        var package = DacPackage.Load(bacpac.FullName);
        var options = new DacDeployOptions();
        var services = new DacServices(ConnectionString);

        services.Deploy(package, databaseName, true, options);
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }

    /// <exception cref="SqlAdapterException" />
    public void DeleteDatabase([NotNull] string databaseName)
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; \r\nDROP DATABASE [{databaseName}]";
          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          command.ExecuteNonQuery();
        }
      }
      catch (SqlException ex)
      {
        if (ex.Class == 14 && ex.State == 5 && ex.Message.Contains("because it does not exist"))
        {
          return;
        }

        throw new SqlAdapterException(ex);
      }
    }

    /// <exception cref="SqlAdapterException" />
    public bool DatabaseExists([NotNull] string databaseName)
    {
      return !string.IsNullOrWhiteSpace(DoGetDatabaseFilePath(databaseName));
    }

    /// <exception cref="SqlAdapterException" />
    [NotNull]
    public IReadOnlyList<string> GetDatabases()
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = "SELECT [Name] FROM sys.Databases";
          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          var list = new List<string>();
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              list.Add(reader.GetString(0));
            }
          }

          return list;
        }
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }
               
    /// <exception cref="SqlAdapterException" />                 
    /// <exception cref="DatabaseDoesNotExistException" />
    public FilePath GetDatabaseFilePath([NotNull] string databaseName)
    {
      var filePath = DoGetDatabaseFilePath(databaseName);
      if (string.IsNullOrWhiteSpace(filePath))
      {
        throw new DatabaseDoesNotExistException(databaseName);
      }

      return new FilePath(filePath);
    }

    private string DoGetDatabaseFilePath([NotNull] string databaseName)
    {
      try
      {
        using (var connection = new SqlConnection(ConnectionString))
        {
          connection.Open();
          var sql = $"SELECT TOP 1 [m].[Physical_Name] " +
            $"FROM [sys].[master_files] [m] " +
            $"INNER JOIN [sys].[Databases] [d] ON [d].[database_id] = [m].[database_id] " +
            $"WHERE [m].[type_desc] = 'ROWS' AND [d].[Name] = '{databaseName}'";

          var command = new SqlCommand
          {
            CommandText = sql,
            Connection = connection,
            CommandTimeout = int.MaxValue
          };

          return (string)command.ExecuteScalar();
        }
      }
      catch (Exception ex)
      {
        throw new SqlAdapterException(ex);
      }
    }
  }
}

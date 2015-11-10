using System.Collections.Generic;
using System.Data.SqlClient;
using MongoDB.Driver;
using SIM.Adapters.MongoDb;
using SIM.Adapters.SqlServer;
using SIM.Pipelines.Install;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines
{
  public static class DeleteDatabasesHelper
  {
    #region Public methods

    public static void Process(IEnumerable<Database> innerDatabases, string rootPath, SqlConnectionStringBuilder connectionString, string instanceName, IPipelineController controller, List<string> done)
    {
      string localDataSource = SqlServerManager.Instance.NormalizeServerName(connectionString.DataSource);

      foreach (Database database in innerDatabases)
      {
        var cstr = database.ConnectionString.ToString();
        if (!SqlServerManager.Instance.IsSqlConnectionString(cstr))
        {
          continue;
        }

        // if the database is attached to remote SQL Server
        string dataSource = SqlServerManager.Instance.NormalizeServerName(database.ConnectionString.DataSource);
        if (!dataSource.EqualsIgnoreCase(localDataSource))
        {
          // user doesn't confirm deleting
          if (controller == null || !controller.Confirm("The '{0}' database seems to be located on the '{1}' remote SQL server instead of the '{2}' local one specified in the Settings dialog. \n\nShould it be deleted as well?".FormatWith(database.RealName, dataSource, localDataSource)))
          {
            continue;
          }
        }

        string fileName = database.FileName;
        if (!string.IsNullOrEmpty(fileName))
        {
          // database is located out of the rootPath folder
          if (!fileName.ToLower().Contains(rootPath))
          {
            // user doesn't confirm deleting
            if (controller == null || !controller.Confirm("The '{0}' database with '{1}' file path is located out of the {2} instance's root path '{3}'. \n\nShould it be deleted as well?".FormatWith(database.RealName, fileName, instanceName, rootPath)))
            {
              continue;
            }
          }
        }

        if (SqlServerManager.Instance.DatabaseExists(database.RealName, connectionString))
        {
          database.Delete();
        }
      }

      SqlServerManager.Instance.DetectDatabases(rootPath, connectionString, name =>
      {
        if (!done.Contains(name))
        {
          SqlServerManager.Instance.DeleteDatabase(name, 
            connectionString);
          done.Add(name);
        }
      });
    }

    public static void Process(IEnumerable<MongoDbDatabase> mongoDatabases, List<string> done)
    {
      if (!Settings.CoreDeleteMongoDatabases.Value)
      {
        return;
      }

      foreach (var database in mongoDatabases)
      {
        var cstr = database.ConnectionString;
        if (!SqlServerManager.Instance.IsMongoConnectionString(cstr))
        {
          continue;
        }

        var pos = cstr.IndexOf('/', @"mongodb://".Length + 1);
        Assert.IsTrue(pos >= 0 && pos < cstr.Length - 1, "Mongo connection string is corrupted: " + cstr);

        var dbName = cstr.Substring(pos + 1);
        var client = new MongoClient();
        var server = client.GetServer();
        server.Connect();
        var db = server.GetDatabase(dbName);
        db.Drop();
        done.Add(database.Name);
      }
    }

    #endregion
  }
}
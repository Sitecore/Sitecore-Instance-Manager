namespace SIM.Adapters
{
  using System;
  using System.IO;
  using System.Linq;
  using JetBrains.Annotations;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.IO.Real;
  using SIM.Services;

  [TestClass]
  public class SqlAdapter_Tests
  {
    private const string DefaultEnvSqlPath = @"C:\Sitecore\etc\sim2\env\default\SqlServer.txt";

    [NotNull]
    private SqlAdapter Adapter { get; } = new SqlAdapter(new SqlConnectionString(File.ReadAllText(DefaultEnvSqlPath)));

    [TestMethod]
    public void DeleteDatabase_MissingDatabase()
    {
      Adapter.DeleteDatabase(GetRandomDatabaseName());
    }

    [TestMethod]
    public void GetDatabaseFilePath_MissingDatabase()
    {
      var databaseName = GetRandomDatabaseName();

      try
      {
        Adapter.GetDatabaseFilePath(databaseName);
      }
      catch (DatabaseDoesNotExistException ex)
      {
        Assert.AreEqual(databaseName, ex.DatabaseName);
        Assert.AreEqual($"Failed to perform an operation with SqlServer. The requested '{databaseName}' database does not exist", ex.Message);

        return;
      }

      Assert.Fail();
    }

    [TestMethod]
    [DeploymentItem("Adapters\\SqlAdapter_Database.dacpac")]
    public void Deploy_Check_Delete_Check()
    {
      var fileSystem = new RealFileSystem();
      var databaseName = GetRandomDatabaseName();
      var dacpac = fileSystem.ParseFile("SqlAdapter_Database.dacpac");
      Assert.AreEqual(true, dacpac.Exists);

      int count;
      try
      {
        Adapter.DeployDatabase(databaseName, dacpac);
        Assert.AreEqual(true, Adapter.DatabaseExists(databaseName));
        Assert.AreEqual(true, !string.IsNullOrEmpty(Adapter.GetDatabaseFilePath(databaseName)));

        var databases = Adapter.GetDatabases();
        Assert.AreEqual(true, databases.Contains(databaseName));

        count = databases.Count;
        Assert.AreEqual(true, count >= 1);
      }
      finally
      {
        Adapter.DeleteDatabase(databaseName);
      }

      Assert.AreEqual(false, Adapter.DatabaseExists(databaseName));

      var newCount = Adapter.GetDatabases().Count;
      Assert.AreEqual(count - 1, newCount);
    }

    [NotNull]
    private static string GetRandomDatabaseName()
    {
      return Guid.NewGuid().ToString("N");
    }
  }
}

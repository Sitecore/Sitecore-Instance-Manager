namespace SIM.Adapters.MongoDb
{
  using MongoDB.Driver;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public class MongoDbManager
  {
    public static MongoDbManager Instance { get; } = new MongoDbManager();

    static MongoDbManager()
    {
    }

    public virtual bool DatabaseExists([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      var client = new MongoClient();
      var server = client.GetServer();
      server.Connect();
      return server.DatabaseExists(name);
    }

    public virtual bool IsMongoConnectionString(string connectionString)
    {
      return connectionString.StartsWith(@"mongodb://");
    }
  }
}
namespace SIM.Adapters.MongoDb
{
  using JetBrains.Annotations;

  public class MongoDbDatabase
  {
    #region Fields

    [NotNull]
    public string ConnectionString { get; }

    [NotNull]
    public string Name { get; }

    #endregion

    #region Constructors

    public MongoDbDatabase(string name, string connectionString)
    {
      Name = name;
      ConnectionString = connectionString;
    }

    #endregion

    #region Public properties              

    [NotNull]
    public string LogicalName
    {
      get
      {
        return ConnectionString.Substring(ConnectionString.LastIndexOf('/') + 1);
      }
    }              

    #endregion
  }
}
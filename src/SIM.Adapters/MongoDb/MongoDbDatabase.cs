namespace SIM.Adapters.MongoDb
{
  using Sitecore.Diagnostics.Base.Annotations;

  public class MongoDbDatabase
  {
    #region Fields

    [NotNull]
    private readonly string connectionString;

    [NotNull]
    private readonly string name;

    #endregion

    #region Constructors

    public MongoDbDatabase(string name, string connectionString)
    {
      this.name = name;
      this.connectionString = connectionString;
    }

    #endregion

    #region Public properties

    [NotNull]
    public string ConnectionString
    {
      get
      {
        return this.connectionString;
      }
    }

    [NotNull]
    public string LogicalName
    {
      get
      {
        return this.connectionString.Substring(this.connectionString.LastIndexOf('/') + 1);
      }
    }

    [NotNull]
    public string Name
    {
      get
      {
        return this.name;
      }
    }

    #endregion
  }
}
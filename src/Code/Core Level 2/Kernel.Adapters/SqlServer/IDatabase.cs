namespace SIM.Adapters.SqlServer
{
  public interface IDatabase
  {
    string Name { get; }

    string ConnectionString { get; }
  }
}
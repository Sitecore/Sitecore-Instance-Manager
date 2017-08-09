namespace SIM.Adapters
{
  using JetBrains.Annotations;

  public class DatabaseDoesNotExistException : SqlAdapterException
  {
    [NotNull]
    public string DatabaseName { get; }

    public DatabaseDoesNotExistException([NotNull] string databaseName)
      : base($"The requested '{databaseName}' database does not exist")
    {
      DatabaseName = databaseName;
    }
  }
}
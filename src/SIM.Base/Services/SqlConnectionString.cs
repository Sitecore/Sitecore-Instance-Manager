namespace SIM.Base.Services
{
  using System.Data.SqlClient;
  using JetBrains.Annotations;

  public sealed class SqlConnectionString : ConnectionString
  {
    [NotNull]
    public SqlConnectionStringBuilder Builder { get; }

    public SqlConnectionString([NotNull] string value)
      : base(value)
    {
      Builder = new SqlConnectionStringBuilder(value);
    }
  }
}
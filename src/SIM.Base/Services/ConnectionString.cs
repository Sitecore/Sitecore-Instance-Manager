namespace SIM.Base.Services
{
  using JetBrains.Annotations;

  public abstract class ConnectionString
  {
    [NotNull]
    public string Value { get; }

    protected ConnectionString([NotNull] string value)
    {
      Value = value;
    }

    [NotNull]
    public static implicit operator string([NotNull] ConnectionString connectionString)
    {
      return connectionString.Value;
    }
  }
}

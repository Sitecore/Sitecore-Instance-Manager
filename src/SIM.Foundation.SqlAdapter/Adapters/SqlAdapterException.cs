namespace SIM.Adapters
{
  using System;
  using JetBrains.Annotations;

  public class SqlAdapterException : Exception
  {
    protected SqlAdapterException([NotNull] string message)
      : base($"Failed to perform an operation with SqlServer. {message}")
    {
    }

    public SqlAdapterException(Exception ex)
      : base("Failed to perform an operation with SqlServer", ex)
    {
    }
  }
}
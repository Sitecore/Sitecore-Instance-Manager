namespace SIM.Instances
{
  using System;

  public class InvalidConfigurationException : NotSupportedException
  {
    #region Constructors

    private InvalidConfigurationException(string message) : base(message)
    {
    }

    #endregion

    #region Public methods

    public static void Assert(bool value, string message)
    {
      if (!value)
      {
        throw new InvalidConfigurationException(message);
      }
    }

    #endregion
  }
}
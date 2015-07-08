#region Usings

using System;

#endregion

namespace SIM.Instances
{
  public class InvalidConfigurationException : NotSupportedException
  {
    private InvalidConfigurationException(string message) : base(message)
    {
    }

    public static void Assert(bool value, string message)
    {
      if(!value)
      {
        throw new InvalidConfigurationException(message);
      }
    }
  }
}

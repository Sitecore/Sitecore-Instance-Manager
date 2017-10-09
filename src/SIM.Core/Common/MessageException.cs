namespace SIM.Core.Common
{
  using System;

  public class MessageException : Exception
  {
    public MessageException(string message) : base(message)
    {
    }
  }
}
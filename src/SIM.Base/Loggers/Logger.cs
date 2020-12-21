using System;
using Sitecore.Diagnostics.Base;

namespace SIM.Loggers
{
  public class Logger : ILogger
  {
    internal Action<string> _WriteLogMessage;

    public Logger(Action<string> doLogMessage)
    {
      Assert.ArgumentNotNull(doLogMessage);
      this._WriteLogMessage = doLogMessage;
    }

    public void Info(string message)
    {
      string text = $"INFO: {message}";
      this._WriteLogMessage(text);
    }
  }
}
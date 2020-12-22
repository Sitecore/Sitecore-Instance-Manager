using System;
using Sitecore.Diagnostics.Base;

namespace SIM.Loggers
{
  public class Logger : ILogger
  {
    internal Action<string> _WriteLogMessage;

    public Logger(Action<string> writeLogMessage)
    {
      Assert.ArgumentNotNull(writeLogMessage);
      this._WriteLogMessage = writeLogMessage;
    }

    public void Info(string message, bool includeSeverityLevel = true)
    {
      string time = DateTime.Now.ToString("HH:mm:ss");
      string text = includeSeverityLevel ? $"[{time}] INFO: {message}" : $"[{time}] {message}";
      this._WriteLogMessage(text);
    }
  }
}
using System;
using Microsoft.Extensions.Logging;

namespace SIM.Core.Logging
{
  public class SitecoreLogger : ILogger
  {
    private readonly LogLevel _logLevel;

    public SitecoreLogger() : this(LogLevel.Information) { }

    public SitecoreLogger(LogLevel level)
    {
      _logLevel = level;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return logLevel >= this._logLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      if (IsEnabled(logLevel))
      {
        switch (logLevel)
        {
          case LogLevel.Information:
            LogInfo(state.ToString(), exception);
            break;
          case LogLevel.Critical:
            LogFatal(state.ToString(), exception);
            break;
          case LogLevel.Trace:
          case LogLevel.Debug:
            LogDebug(state.ToString(), exception);
            break;
          case LogLevel.Error:
            LogError(state.ToString(), exception);
            break;
          case LogLevel.Warning:
            LogWarn(state.ToString(), exception);
            break;
          case LogLevel.None:
          default:
            return;
        }
      }
    }

    private void LogWarn(string message, Exception ex)
    {
      if (ex != null)
      {
        Sitecore.Diagnostics.Logging.Log.Warn(ex, message);
      }
      else
      {
        Sitecore.Diagnostics.Logging.Log.Warn(message);
      }
    }

    private void LogError(string message, Exception ex)
    {
      if (ex != null)
      {
        Sitecore.Diagnostics.Logging.Log.Error(ex, message);
      }
      else
      {
        Sitecore.Diagnostics.Logging.Log.Error(message);
      }
    }

    private void LogDebug(string message, Exception ex)
    {
      if (ex != null)
      {
        Sitecore.Diagnostics.Logging.Log.Debug(ex, message);
      }
      else
      {
        Sitecore.Diagnostics.Logging.Log.Debug(message);
      }
    }

    private void LogFatal(string message, Exception ex)
    {
      if (ex != null)
      {
        Sitecore.Diagnostics.Logging.Log.Fatal(ex, message);
      }
      else
      {
        Sitecore.Diagnostics.Logging.Log.Fatal(message);
      }
    }

    protected void LogInfo(string message, Exception ex)
    {
      if (ex != null)
      {
        Sitecore.Diagnostics.Logging.Log.Info(ex, message);
      }
      else
      {
        Sitecore.Diagnostics.Logging.Log.Info(message);
      }
    }
  }
}
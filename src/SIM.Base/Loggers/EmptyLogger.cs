namespace SIM.Loggers
{
  public class EmptyLogger : ILogger
  {
    public void Info(string message, bool includeSeverityLevel = true)
    {
    }

    public void Warn(string message, bool includeSeverityLevel = true)
    {
    }

    public void Error(string message, bool includeSeverityLevel = true)
    {
    }
  }
}
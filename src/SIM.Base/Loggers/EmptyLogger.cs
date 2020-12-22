namespace SIM.Loggers
{
  public class EmptyLogger : ILogger
  {
    public void Info(string message, bool includeSeverityLevel = true)
    {
    }
  }
}
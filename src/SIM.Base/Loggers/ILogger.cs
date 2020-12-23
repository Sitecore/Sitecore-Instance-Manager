namespace SIM.Loggers
{
  public interface ILogger
  {
    void Info(string message, bool includeSeverityLevel = true);
  }
}
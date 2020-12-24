namespace SIM.Loggers
{
  public interface ILogger
  {
    void Info(string message, bool includeSeverityLevel = true);

    void Warn(string message, bool includeSeverityLevel = true);
  }
}
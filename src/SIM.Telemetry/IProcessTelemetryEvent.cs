using System.Net.Http;

namespace SIM.Telemetry
{
  public interface IProcessTelemetryEvent
  {
    void ProcessEvent(IConfiguration context);
    TelemetryEvent Event { get;}
  }
}
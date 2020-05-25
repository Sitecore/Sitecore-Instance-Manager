namespace SIM.Telemetry.Providers
{
  using SIM.Telemetry.Models;
  using System.Collections.Generic;

  public abstract class TelemetryProviderBase
  {
    protected virtual bool IsEnabled { get; set; }

    public abstract void TrackEvent(string telemetryEvent, Dictionary<string, string> customEventData, TelemetryEventContext telemetryEventContext);
  }
}

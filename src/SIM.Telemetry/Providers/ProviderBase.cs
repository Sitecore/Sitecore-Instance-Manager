namespace SIM.Telemetry.Providers
{
  using SIM.Telemetry.Models;

  public abstract class TelemetryProviderBase
  {
    protected virtual bool IsEnabled { get; set; }

    public abstract void TrackEvent(TelemetryEvent telemetryEvent, TelemetryEventContext telemetryEventContext);
  }
}

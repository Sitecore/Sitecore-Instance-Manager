namespace SIM.Telemetry.Models
{
  using System;

  public class TelemetryEventContext
  {
    public Guid ApplicationID { get; set; }

    public Guid DeviceId { get; set; }

    public string AppVersion { get; set; }

    public string OperatingSystem { get; set; }

    public string Language { get; set; }

    public int ScreenWidth { get; set; }

    public int ScreenHeight { get; set; }
  }
}

namespace SIM.Telemetry.Models
{
  using System;
  public class AppUtilizationModel
  {
    public Guid DeviceId { get; set; }

    public Guid ApplicationId { get; set; }

    public string AppVersion { get; set; }

    public string OperatingSystem { get; set; }

    public string Event { get; set; }

    public string Language { get; set; }

    public int ScreenWidth { get; set; }

    public int ScreenHeight { get; set; }

    public static AppUtilizationModel GetInstance(TelemetryEvent telemetryEvent, TelemetryEventContext eventContext)
    {
      var model = new AppUtilizationModel()
      {
        Event = telemetryEvent.ToString(),
        ApplicationId = eventContext.ApplicationID,
        AppVersion = eventContext.AppVersion,
        DeviceId = eventContext.DeviceId,
        Language = eventContext.Language,
        OperatingSystem = eventContext.OperatingSystem,
        ScreenWidth = eventContext.ScreenWidth,
        ScreenHeight = eventContext.ScreenHeight
      };

      return model;
    }
  }
}

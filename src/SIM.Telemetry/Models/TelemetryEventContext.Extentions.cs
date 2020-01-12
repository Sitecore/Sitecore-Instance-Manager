namespace SIM.Telemetry.Models
{
  using KB.Telemetry;

  public static class Extentions
  {
    public static AppUtilizationModel ToAppUtilizationModel(this TelemetryEventContext eventContext, TelemetryEvent telemetryEvent)
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

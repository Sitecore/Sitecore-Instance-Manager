namespace SIM.Telemetry.Models
{
  using JetBrains.Annotations;
  using KB.Telemetry;
  using System.Collections.Generic;

  public static class Extentions
  {
    public static AppUtilizationModel ToAppUtilizationModel(this TelemetryEventContext eventContext, string telemetryEvent, [CanBeNull] Dictionary<string, string> customEventData)
    {
      var model = new AppUtilizationModel()
      {
        Event = telemetryEvent.ToString(),
        CustomEventData = customEventData ?? new Dictionary<string, string>(),
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

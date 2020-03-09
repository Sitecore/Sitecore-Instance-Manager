namespace SIM.Telemetry.Models
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
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

    public TelemetryEventContext(Guid applicationId, Guid deviceId, [CanBeNull]string appVersion)
    {
      Assert.ArgumentNotNullOrEmpty(appVersion, "appVersion");

      ApplicationID = applicationId;
      DeviceId = deviceId;
      AppVersion = appVersion;

      OperatingSystem = Environment.OSVersion.ToString();
      Language = AnalyticsHelper.GetCurrentUICulture();
      ScreenWidth = AnalyticsHelper.GetScreenWidth();
      ScreenHeight = AnalyticsHelper.GetScreenHeight();
    }
  }
}

namespace SIM.Telemetry.Models
{
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
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

    protected readonly ILogger _logger;

    public TelemetryEventContext(Guid applicationId, 
      Guid deviceId, 
      [CanBeNull] string appVersion, 
      [CanBeNull] ILogger logger)
    {
      if (string.IsNullOrWhiteSpace(appVersion)) throw new ArgumentNullException(nameof(appVersion));
      if (logger == null) throw new ArgumentNullException(nameof(logger));

      _logger = logger;
      ApplicationID = applicationId;
      DeviceId = deviceId;
      AppVersion = appVersion;

      OperatingSystem = Environment.OSVersion.ToString();
      Language = AnalyticsHelper.GetCurrentUICulture(this._logger);
      ScreenWidth = AnalyticsHelper.GetScreenWidth();
      ScreenHeight = AnalyticsHelper.GetScreenHeight();
    }
  }
}
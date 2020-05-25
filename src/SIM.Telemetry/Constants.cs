using System;

namespace SIM.Telemetry
{
  public enum TelemetryEvent { AppRun, AppExit }

  public static class Constants
  {
    public static Guid SitecoreInstanceManagerAppId => KB.Telemetry.Constants.SitecoreInstanceManagerAppId;

    public static string KBProviderBaseAddress => KB.Telemetry.Constants.KBProviderBaseAddress;
  }
}

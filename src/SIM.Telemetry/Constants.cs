namespace SIM.Telemetry
{
    using System.Diagnostics;

    public enum TelemetryEvent { AppRun }

    public static class Constants
    {
        public static string SitecoreInstanceManagerAppId => "{10DE394E-2FAD-4145-870C-63248039DA44}";

#if DEBUG
    public static string KBProviderBaseAddress => "http://localhost:8080/services/tracking/trackapputilization";
#endif

#if !DEBUG
        public static string KBProviderBaseAddress => "https://kb.sitecore.net/services/tracking/trackapputilization";
#endif
    }
}

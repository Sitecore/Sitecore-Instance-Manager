namespace SIM.Core.Common
{
  using System;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.Channel;
  using Microsoft.ApplicationInsights.Extensibility;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class Analytics
  {
    [CanBeNull]
    private static TelemetryClient client;

    public static void Start()
    {
      if (CoreApp.DoNotTrack())
      {
        return;
      }

      Log.Debug("Insights - starting");

      try
      {
        var configuration = new TelemetryConfiguration
        {
          InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359", 
          TelemetryChannel = new PersistenceChannel()
        };

        var tc = new TelemetryClient(configuration)
        {
          InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359"
        };

        client = tc;
        try
        {
          tc.Context.Component.Version = string.IsNullOrEmpty(ApplicationManager.AppVersion) ? "0.0.0.0" : ApplicationManager.AppVersion;
          tc.Context.Session.Id = Guid.NewGuid().ToString();
          tc.Context.User.Id = Environment.MachineName + "\\" + Environment.UserName;
          tc.Context.User.AccountId = CoreApp.GetCookie();
          tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
          tc.TrackEvent("Start");
          tc.Flush();
        }
        catch (Exception ex)
        {
          tc.TrackException(ex);
          Log.Error(ex, "Error in app insights");
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error in app insights");
      }

      Log.Debug("Insights - started");
    }

    public static void TrackEvent([NotNull] string eventName)
    {
      Assert.ArgumentNotNull(eventName, "eventName");

      var tc = client;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent(eventName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during event tracking: {0}", eventName);
      }
    }

    public static void Flush()
    {
      var tc = client;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent("Exit");

        tc.Flush();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during flushing");
      }
    }
  }
}

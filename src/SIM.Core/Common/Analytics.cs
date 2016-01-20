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
    private static TelemetryClient telemetryClient;

    public static void Start()
    {
      if (CoreApp.DoNotTrack())
      {
        return;
      }

      Log.Debug("Insights - starting");

      try
      {
        var configuration = TelemetryConfiguration.Active;
        Assert.IsNotNull(configuration, "configuration");

        configuration.TelemetryChannel = new PersistenceChannel("Sitecore Instance Manager");
        configuration.InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359";
        
        var client = new TelemetryClient(configuration)
        {
          InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359"
        };

        Analytics.telemetryClient = client;
        try
        {
          // ReSharper disable PossibleNullReferenceException
          client.Context.Component.Version = string.IsNullOrEmpty(ApplicationManager.AppVersion) ? "0.0.0.0" : ApplicationManager.AppVersion;
          client.Context.Session.Id = Guid.NewGuid().ToString();
          client.Context.User.Id = Environment.MachineName + "\\" + Environment.UserName;
          client.Context.User.AccountId = CoreApp.GetCookie();
          client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
          // ReSharper restore PossibleNullReferenceException
          client.TrackEvent("Start");
          client.Flush();
        }
        catch (Exception ex)
        {
          client.TrackException(ex);
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

      var tc = telemetryClient;
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
      var tc = telemetryClient;
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

namespace SIM.Core.Common
{
  using System;
  using System.IO;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.Channel;
  using Microsoft.ApplicationInsights.Extensibility;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.FileSystem;

  public static class Analytics
  {
    [CanBeNull]
    private static TelemetryClient telemetryClient;

    public static void Start()
    {
      if (DoNotTrack())
      {
        return;
      }

      Log.Debug(string.Format("Insights - starting"));

      try
      {
        var configuration = TelemetryConfiguration.Active;
        Assert.IsNotNull(configuration, nameof(configuration));

        configuration.TelemetryChannel = new PersistenceChannel("Sitecore Instance Manager");
        configuration.InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359";

        var client = new TelemetryClient(configuration)
        {
          InstrumentationKey = "1447f72f-2d39-401b-91ac-4d5c502e3359"
        };

        telemetryClient = client;
        try
        {
          // ReSharper disable PossibleNullReferenceException
          client.Context.Component.Version = string.IsNullOrEmpty(ApplicationManager.AppVersion) ? "0.0.0.0" : ApplicationManager.AppVersion;
          client.Context.Session.Id = Guid.NewGuid().ToString();
          client.Context.User.Id = Environment.MachineName + "\\" + Environment.UserName;
          client.Context.User.AccountId = GetCookie();
          client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
          // ReSharper restore PossibleNullReferenceException
          client.TrackEvent("Start");
          client.Flush();
        }
        catch (Exception ex)
        {
          client.TrackException(ex);
          Log.Error(ex, string.Format("Error in app insights"));
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, string.Format("Error in app insights"));
      }

      Log.Debug(string.Format("Insights - started"));
    }

    public static void TrackEvent([NotNull] string eventName)
    {
      Assert.ArgumentNotNull(eventName, nameof(eventName));

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
        Log.Error(ex, $"Error during event tracking: {eventName}");
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
        Log.Error(ex, string.Format("Error during flushing"));
      }
    }
    public static bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return FileSystem.Local.File.Exists(path);
    }

    [NotNull]
    public static string GetCookie()
    {
      var tempFolder = ApplicationManager.TempFolder;
      var path = Path.Combine(tempFolder, "cookie.txt");
      if (Directory.Exists(tempFolder))
      {
        if (FileSystem.Local.File.Exists(path))
        {
          var cookie = FileSystem.Local.File.ReadAllText(path);
          if (!string.IsNullOrEmpty(cookie))
          {
            return cookie;
          }

          try
          {
            FileSystem.Local.File.Delete(path);
          }
          catch (Exception ex)
          {
            Log.Error(ex, string.Format("Cannot delete cookie file"));
          }
        }
      }
      else
      {
        Directory.CreateDirectory(tempFolder);
      }

      var newCookie = Guid.NewGuid().ToShortGuid();
      try
      {
        FileSystem.Local.File.WriteAllText(path, newCookie);
      }
      catch (Exception ex)
      {
        Log.Error(ex, string.Format("Cannot write cookie"));
      }

      return newCookie;
    }
  }
}
namespace SIM.Telemetry
{
  using System;
  using System.Collections.Generic;
  using SIM.Telemetry.Models;
  using Sitecore.Diagnostics.Logging;
  using JetBrains.Annotations;
  using SIM.Telemetry.Providers;
  using System.Threading.Tasks;
  using Sitecore.Diagnostics.Base;

  public class Analytics
  {
    #region Private Fields
    private static List<TelemetryProviderBase> _telemetryProviders;
    #endregion

    #region  Private Properties
    private static List<TelemetryProviderBase> TelemetryProviders
    {
      get
      {
        if (_telemetryProviders == null)
        {
          return _telemetryProviders = new List<TelemetryProviderBase>();
        }

        return _telemetryProviders;
      }
    }

    private static bool IsTrackingEnabled { get; set; }

    private static TelemetryEventContext EventContext { get; set; }
    #endregion

    #region Public Properties
    public static bool IsInitialized { get; private set; }
    #endregion

    #region Public Methods
    public static void Initialize([NotNull] TelemetryEventContext telemetryEventContext, bool trackingEnabled)
    {
      EventContext = telemetryEventContext;
      IsTrackingEnabled = trackingEnabled;

      if (telemetryEventContext.DeviceId != Guid.Empty)
      {
        IsInitialized = true;
      }
      else
      {
        IsInitialized = false;

        Log.Debug($"'SIM.Telemetry.Analytics' initialization failed. 'DeviceId' is empty.");
      }
    }

    public static void Track(TelemetryEvent tEvent)
    {
      if (!IsInitialized)
      {
        Log.Debug($"'Analytics' manager has not been initialized. Telemetry event '{tEvent.ToString()}' will not be processed.");

        return;
      }

      if (!IsTrackingEnabled)
      {
        Log.Debug($"Analytics is disabled. Telemetry event '{tEvent.ToString()}' will not be processed.");

        return;
      }

      if (TelemetryProviders.Count <= 0)
      {
        Log.Debug($"Telemetry providers list is empty. Telemetry event '{tEvent.ToString()}' will not be processed.");

        return;
      }

      Task.Run(() => DoTrackEvent(tEvent));
    }

    public static void RegisterProvider(TelemetryProviderBase telemetryProvider)
    {
      Assert.ArgumentNotNull(telemetryProvider, "telemetryProvider");

      TelemetryProviders.Add(telemetryProvider);
    }
    #endregion

    #region Private Methods
    private static void DoTrackEvent(TelemetryEvent telemetryEvent)
    {
      var providers = TelemetryProviders.ToArray();

      foreach (var provider in providers)
      {
        try
        {
          provider.TrackEvent(telemetryEvent, EventContext);
        }
        catch (Exception ex)
        {
          Log.Debug($"Telemetry event '{telemetryEvent.ToString()}' has not been processed by '{provider.GetType()}' provider.{Environment.NewLine}" +
            $"Message: {ex.Message}{Environment.NewLine}" +
            $"{ex.StackTrace}{Environment.NewLine}");

          return;
        }
      }
    }
    #endregion
  }
}
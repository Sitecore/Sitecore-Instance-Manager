namespace SIM.Telemetry
{
  using System;
  using System.Collections.Generic;
  using SIM.Telemetry.Models;
  using JetBrains.Annotations;
  using SIM.Telemetry.Providers;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Logging;

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

    private static ILogger _logger;

    #region Public Properties
    public static bool IsInitialized { get; private set; }
    #endregion

    #region Public Methods
    public static void Initialize([CanBeNull] TelemetryEventContext telemetryEventContext, 
      bool trackingEnabled,
      [CanBeNull]ILogger logger)
    {      
      if (telemetryEventContext == null) throw new ArgumentNullException(nameof(telemetryEventContext));
      if (logger == null) throw new ArgumentNullException(nameof(logger));

      EventContext = telemetryEventContext;
      IsTrackingEnabled = trackingEnabled;
      _logger = logger;

      if (telemetryEventContext.DeviceId != Guid.Empty)
      {
        IsInitialized = true;
      }
      else
      {
        IsInitialized = false;

        _logger.LogDebug($"'Telemetry.Analytics' initialization failed. 'DeviceId' is empty.");
      }

      _logger.LogInformation("'Telemetry.Analytics' has been initialized successfully");
    }

    public static void Track(TelemetryEvent tEvent)
    {
      Track(tEvent.ToString(), new Dictionary<string, string>());
    }

    public static void Track(string tEvent)
    {
      Track(tEvent, new Dictionary<string, string>());
    }

    public static void Track(TelemetryEvent tEvent, Dictionary<string, string> eventCustomData)
    {
      Track(tEvent.ToString(), eventCustomData);
    }

    public static void Track(string tEvent, Dictionary<string, string> eventCustomData)
    {
      if (!IsInitialized)
      {
        _logger.LogDebug($"'Analytics' manager has not been initialized. Telemetry event '{tEvent}' will not be processed.");

        return;
      }

      if (!IsTrackingEnabled)
      {
        _logger.LogDebug($"Analytics is disabled. Telemetry event '{tEvent}' will not be processed.");

        return;
      }

      if (TelemetryProviders.Count <= 0)
      {
        _logger.LogDebug($"Telemetry providers list is empty. Telemetry event '{tEvent.ToString()}' will not be processed.");

        return;
      }

      Task.Run(() => DoTrackEvent(tEvent, eventCustomData));
    }

    public static void RegisterProvider(TelemetryProviderBase telemetryProvider)
    {
      if (telemetryProvider == null) throw new ArgumentNullException(nameof(telemetryProvider));

      TelemetryProviders.Add(telemetryProvider);
    }
    #endregion

    #region Private Methods
    private static void DoTrackEvent(string telemetryEvent, Dictionary<string, string> eventCustomData)
    {
      var providers = TelemetryProviders.ToArray();

      foreach (var provider in providers)
      {
        try
        {
          provider.TrackEvent(telemetryEvent, eventCustomData, EventContext);
        }
        catch (Exception ex)
        {          
          _logger.LogDebug(ex, $"Telemetry event '{telemetryEvent.ToString()}' has not been processed by '{provider.GetType()}' provider.{Environment.NewLine}");

          return;
        }
      }
    }
    #endregion
  }
}
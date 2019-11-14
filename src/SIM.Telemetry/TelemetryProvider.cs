namespace SIM.Telemetry
{
  using Sitecore.Diagnostics.Logging;
  using System;
  using System.Collections.Generic;


  public class TelemetryProvider
  {
    private static Dictionary<TelemetryEvent, IProcessTelemetryEvent> processors = new Dictionary<TelemetryEvent, IProcessTelemetryEvent>();
    private static bool _isInitialized;

    // Add new telemetry event processors to the array
    IProcessTelemetryEvent[] ProcessorsList => new IProcessTelemetryEvent[]
    {
      new AppRunEventProcessor()
    };


    public virtual void Track(IConfiguration config, TelemetryEvent tEvent)
    {
      EnsureInitialized();

      if (processors.ContainsKey(tEvent))
      {
        try
        {
          processors[tEvent].ProcessEvent(config);
        }
        catch (Exception ex)
        {
          Log.Error($"Exception occurred during sendning '{tEvent}' telemetry event. {ex.Message}, {ex.StackTrace}");
        }
      }
      else
      {
        Log.Debug($"There is no defined processor for '{tEvent}'");
      }
    }

    private void EnsureInitialized()
    {
      if (_isInitialized)
      {
        return;
      }
      else
      {
        Initialize();
      }
    }

    private void Initialize()
    {
      var dictionary = new Dictionary<TelemetryEvent, IProcessTelemetryEvent>();

      foreach (var processor in ProcessorsList)
      {
        dictionary[processor.Event] = processor;
      }

      processors = dictionary;

      _isInitialized = true;
    }
  }
}
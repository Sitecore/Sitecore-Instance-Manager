namespace SIM.Telemetry
{
  using System;
  using System.Net.Http;

  public enum TelemetryEvent { AppRun }

  public class TelemetryClient
  {
    private static HttpClient client => new HttpClient();
    private TelemetryProvider Provider => new TelemetryProvider();
    private Guid ApplicationId { get; }
    public bool IsEnabled { get; }
    private string BaseAddress { get; }
    private string ConfigFolder { get; }

    public TelemetryClient(Guid appId, string baseAddress, bool enabled, string configFolder)
    {
      ApplicationId = appId;

      IsEnabled = enabled;

      BaseAddress = baseAddress;

      ConfigFolder = configFolder;
    }

    public void Track(TelemetryEvent tEvent)
    {
      if (!IsEnabled)
      {
        return;
      }

      var configuration = new Configuration() {
        ApplicationId = ApplicationId,
        BaseAddress = BaseAddress,
        Client = client,
        ConfigFolder = ConfigFolder
      };

      Provider.Track(configuration, tEvent);    
    }
  }
}

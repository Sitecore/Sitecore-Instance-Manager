namespace SIM.Telemetry.Providers
{
  using System;
  using System.Net.Http;
  using System.Text;
  using KB.Telemetry;
  using Newtonsoft.Json;
  using SIM.Telemetry.Models;
  using Sitecore.Diagnostics.Logging;

  public class KnowledgeBaseProvider : TelemetryProviderBase
  {
    #region Protected Properties
    protected static HttpClient Client => new HttpClient();
    protected string BaseAddress { get; }
    #endregion

    #region Constructors
    private KnowledgeBaseProvider() { }

    public KnowledgeBaseProvider(KBProviderConfiguration configuration) : this(configuration, enabled: true) { }

    public KnowledgeBaseProvider(KBProviderConfiguration configuration, bool enabled)
    {
      IsEnabled = enabled;

      BaseAddress = configuration.BaseAddress;      
    }
    #endregion

    #region Public Methods
    public override void TrackEvent(TelemetryEvent telemetryEvent, TelemetryEventContext eventContext)
    {
      var appUtilizationModel = eventContext.ToAppUtilizationModel(telemetryEvent);

      try
      {
        var utilizationModelJson = JsonConvert.SerializeObject(appUtilizationModel);

        var stringContent = new StringContent(utilizationModelJson, Encoding.UTF8, "application/json");

        var response = Client.PutAsync(this.BaseAddress, stringContent).Result;

        response.EnsureSuccessStatusCode();
      }
      catch (HttpRequestException e)
      {
        var errorMessage = $"Exception Caught. {Environment.NewLine}Message :{e.Message}{Environment.NewLine}" +
          $"{e.StackTrace}";

        Log.Debug(errorMessage);
      }
    }
    #endregion
  }
}
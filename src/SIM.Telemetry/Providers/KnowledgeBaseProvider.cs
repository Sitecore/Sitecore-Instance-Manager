namespace SIM.Telemetry.Providers
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Text;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
  using Newtonsoft.Json;
  using SIM.Telemetry.Models;

  public class KnowledgeBaseProvider : TelemetryProviderBase
  {
    #region Protected Properties
    protected static HttpClient Client => new HttpClient();
    protected string BaseAddress { get; }

    protected ILogger _logger;
    #endregion

    #region Constructors
    private KnowledgeBaseProvider() { }

    public KnowledgeBaseProvider(
      KBProviderConfiguration configuration,
      ILogger logger
      ) : this(configuration, true, logger) { }

    public KnowledgeBaseProvider(
      [CanBeNull] KBProviderConfiguration configuration,
      bool enabled,
      [CanBeNull] ILogger logger)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));
      if (logger == null) throw new ArgumentNullException(nameof(logger));

      IsEnabled = enabled;

      BaseAddress = configuration.BaseAddress;      

      _logger = logger;
    }
    #endregion

    #region Public Methods
    public override void TrackEvent(string telemetryEvent, Dictionary<string, string> customEventData, TelemetryEventContext eventContext)
    {
      var appUtilizationModel = eventContext.ToAppUtilizationModel(telemetryEvent, customEventData);

      try
      {
        var utilizationModelJson = JsonConvert.SerializeObject(appUtilizationModel);

        var stringContent = new StringContent(utilizationModelJson, Encoding.UTF8, "application/json");

        var response = Client.PutAsync(this.BaseAddress, stringContent).Result;

        response.EnsureSuccessStatusCode();
      }
      catch (HttpRequestException e)
      {
        var errorMessage = $"KnowledgeBaseProvider: telemetry tracking failed.";

        _logger.LogDebug(e, errorMessage);
      }
    }
    #endregion
  }
}
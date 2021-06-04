using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.UserControls.SitecoreAuthentication
{
  public static class SitecoreIdServerAuth
  {
    public static HttpStatusCode CurrentHttpStatusCode { get; private set; }

    public static async Task<string> GetToken(string idServerUri, string userName, string password)
    {
      using (HttpClient authClient = new HttpClient())
      {
        authClient.BaseAddress = new Uri(idServerUri);
        authClient.DefaultRequestHeaders.Accept.Clear();
        authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new FormUrlEncodedContent(new[]
        {
          new KeyValuePair<string, string>("client_id", "SitecorePassword"),
          new KeyValuePair<string, string>("client_secret", "SIF-Default"),
          new KeyValuePair<string, string>("grant_type", "password"),
          new KeyValuePair<string, string>("username", userName),
          new KeyValuePair<string, string>("password", password)
        });

        try
        {
          var response = await authClient.PostAsync("connect/token", content);
          CurrentHttpStatusCode = response.StatusCode;
          if (response.IsSuccessStatusCode)
          {
            var result = JsonConvert.DeserializeObject<TokenResponse>(response.Content.ReadAsStringAsync().Result);
            if (!string.IsNullOrEmpty(result.access_token))
            {
              return $"Bearer {result.access_token}";
            }
            CurrentHttpStatusCode = HttpStatusCode.NoContent;
          }
        }
        catch (OperationCanceledException ex)
        {
          Log.Error(ex, ex.Message);
          CurrentHttpStatusCode = HttpStatusCode.RequestTimeout;
        }
        catch (Exception ex)
        {
          Log.Error(ex, ex.Message);
          CurrentHttpStatusCode = HttpStatusCode.InternalServerError;
        }

        return null;
      }
    }

    private struct TokenResponse
    {
      public string access_token { get; set; }

      public long expires_in { get; set; }

      public string token_type { get; set; }
    }
  }
}
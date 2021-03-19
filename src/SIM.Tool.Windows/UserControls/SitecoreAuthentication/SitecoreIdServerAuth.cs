using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SIM.Tool.Windows.UserControls.SitecoreAuthentication
{
  public static class SitecoreIdServerAuth
  {
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
        var response = await authClient.PostAsync("connect/token", content);
        var result = JsonConvert.DeserializeObject<TokenResponse>(response.Content.ReadAsStringAsync().Result);
        if (string.IsNullOrEmpty(result.access_token))
        {
          return null;
        }
        return $"Bearer {result.access_token}";
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
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SIM.Tool.Windows.UserControls.Helpers
{
  public static class SitecoreIdServerAuth
  {
    public static async Task<string> GetToken(string idServerUri, string clientId, string clientSecret, string grantType, string username, string password)
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(idServerUri);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new FormUrlEncodedContent(new[]
        {
          new KeyValuePair<string, string>("client_id", clientId),
          new KeyValuePair<string, string>("client_secret", clientSecret),
          new KeyValuePair<string, string>("grant_type", grantType),
          new KeyValuePair<string, string>("username", username),
          new KeyValuePair<string, string>("password", password)
        });
        var response = await client.PostAsync("connect/token", content);
        var result = JsonConvert.DeserializeObject<TokenResponse>(response.Content.ReadAsStringAsync().Result);
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
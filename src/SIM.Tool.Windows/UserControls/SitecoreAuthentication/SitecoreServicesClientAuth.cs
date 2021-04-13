using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.UserControls.SitecoreAuthentication
{
  public static class SitecoreServicesClientAuth
  {
    public static HttpStatusCode CurrentHttpStatusCode { get; private set; }

    public static async Task<string> GetCookie(string idServerUri, string userName, string password)
    {
      CookieContainer cookieContainer = new CookieContainer();
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        CookieContainer = cookieContainer,
        UseCookies = true
      };

      using (HttpClient authClient = new HttpClient(httpClientHandler))
      {
        Uri uri = new Uri(idServerUri + "/sitecore/api/ssc/auth/login");

        authClient.BaseAddress = uri;
        authClient.DefaultRequestHeaders.Accept.Clear();
        authClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        GetDomainAndName(userName, out string domain, out string name);

        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
        {
          new KeyValuePair<string, string>("domain", domain),
          new KeyValuePair<string, string>("username", name),
          new KeyValuePair<string, string>("password", password)
        });

        try
        {
          var response = await authClient.PostAsync(uri, content);
          Cookie authCookie = cookieContainer.GetCookies(uri).Cast<Cookie>().FirstOrDefault(x => x.Name == ".ASPXAUTH");
          CurrentHttpStatusCode = response.StatusCode;
          if (response.IsSuccessStatusCode)
          {
            return authCookie?.ToString();
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

    private static void GetDomainAndName(string userName, out string domain, out string username)
    {
      domain = null;
      username = null;
      if (!string.IsNullOrEmpty(userName))
      {
        int charLocation = userName.IndexOf("\\", StringComparison.InvariantCultureIgnoreCase);

        if (charLocation > 0)
        {
          domain = userName.Substring(0, charLocation);
          username = userName.Substring(charLocation + 1);
        }
      }
    }
  }
}
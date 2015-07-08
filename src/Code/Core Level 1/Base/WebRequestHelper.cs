using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;

namespace SIM.Base
{
  public static class WebRequestHelper
  {
    public const int Second = 1000;
    public const int Minute = 60 * Second;
    public const int Hour = 60 * Minute;

    /// <summary>
    /// Downloads web resource by specified URL.
    /// </summary>
    /// <param name="url">The URL of web resource.</param>
    /// <param name="timeout">The timeout of request (milliseconds).</param>
    /// <param name="readWriteTimeout">Read-write timeout</param>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ProtocolViolationException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns>The web resource output (e.g. HTML).</returns>
    public static string DownloadString(string url, int? timeout = null, int? readWriteTimeout = null)
    {
      using (var response = RequestAndGetResponse(url, timeout, readWriteTimeout))
      {
        Assert.IsNotNull(response, "No response provided");
        var stream = response.GetResponseStream();
        Assert.IsNotNull(stream, "No response stream provided");
        using (var streamReader = new StreamReader(stream))
        {
          return streamReader.ReadToEnd();
        }
      }
    }

    /// <summary>
    /// Requests web resource by specified URL and provides its response.
    /// </summary>
    /// <param name="url">The URL of web resource.</param>
    /// <param name="timeout">The timeout of request (milliseconds).</param>
    /// <param name="readWriteTimeout">Read-write timeout</param>
    /// <param name="cookies">The cookies that will be send with request.</param>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ProtocolViolationException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns>The web resource response.</returns>
    public static HttpWebResponse RequestAndGetResponse(string url, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      return RequestAndGetResponse(new Uri(url), timeout, readWriteTimeout, cookies);
    }

    /// <summary>
    /// Requests web resource by specified URI and provides its response.
    /// </summary>
    /// <param name="uri">The URI of web resource.</param>
    /// <param name="timeout">The timeout of request (milliseconds).</param>
    /// <param name="readWriteTimeout">Read-write timeout</param>
    /// <param name="cookies">The cookies that will be send with request.</param>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ProtocolViolationException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns>The web resource response.</returns>
    public static HttpWebResponse RequestAndGetResponse(Uri uri, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      var webRequest = CreateRequest(uri, timeout, readWriteTimeout, cookies);

      return webRequest.GetResponse() as HttpWebResponse;
    }

    private static HttpWebRequest CreateRequest(string url, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      return CreateRequest(new Uri(url), timeout, readWriteTimeout, cookies);
    }

    /// <summary>
    /// Creates a request to the web resource by specified URI.
    /// </summary>
    /// <param name="url">The URL of web resource.</param>
    /// <param name="timeout">The timeout of request (milliseconds).</param>
    /// <param name="readWriteTimeout">Read-write timeout</param>
    /// <param name="cookies">The cookies that will be send with request.</param>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns>The web resource request object.</returns>
    private static HttpWebRequest CreateRequest(Uri url, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      var webRequest = (HttpWebRequest)WebRequest.Create(url);
      webRequest.Timeout = timeout ?? Settings.CoreWebDownloadConnectionTimeout.Value;
      webRequest.ReadWriteTimeout = readWriteTimeout ?? Settings.CoreWebDownloadTimeoutMinutes.Value * Minute;
      if (cookies != null)
      {
        webRequest.Headers.Add(HttpRequestHeader.Cookie, cookies);
      }

      return webRequest;
    }

    public static void DownloadFile(string url, string destFileName)
    {
      DownloadFile(new Uri(url), destFileName);
    }

    /// <summary>
    /// Downloads file into specified location
    /// </summary>
    /// <param name="url">Link to the remote file</param>
    /// <param name="destFileName">Destination file path.</param>
    /// <param name="timeout">Connection timeout in milliseconds</param>
    /// <param name="readWriteTimeout">Read-write timeout</param>
    /// <param name="cookies">Cookies for request</param>
    /// <returns></returns>
    public static bool DownloadFile(Uri url, string destFileName, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      using (new ProfileSection("Download file", typeof(WebRequestHelper)))
      {
        ProfileSection.Argument("url", url);
        ProfileSection.Argument("destFileName", destFileName);

        var response = RequestAndGetResponse(url, timeout, readWriteTimeout, cookies);
        using (var responseStream = response.GetResponseStream())
        {
          try
          {
            DownloadFile(destFileName, responseStream);

            ProfileSection.Result("Done");
            return true;
          }
          catch (OperationCanceledException)
          {
            FileSystem.Local.File.Delete(destFileName);

            ProfileSection.Result("Canceled");
            return false;
          }
          catch (Exception ex)
          {
            throw new InvalidOperationException("Downloading failed with exception", ex);
          }
        }
      }
    }

    public static bool DownloadFile(Uri url, string destFileName, HttpWebResponse response, CancellationToken? token = null, Action<int> indicateProgress = null)
    {
      using (new ProfileSection("Download file", typeof(WebRequestHelper)))
      {
        ProfileSection.Argument("url", url);
        ProfileSection.Argument("destFileName", destFileName);
        ProfileSection.Argument("response", response);
        ProfileSection.Argument("token", token);
        ProfileSection.Argument("indicateProgress", indicateProgress);

        using (var responseStream = response.GetResponseStream())
        {
          try
          {
            Assert.ArgumentNotNull(responseStream, "responseStream");
            responseStream.ReadTimeout = Settings.CoreWebDownloadTimeoutMinutes.Value * Minute;
            DownloadFile(destFileName, responseStream, token, indicateProgress);

            ProfileSection.Result("Done");
            return true;
          }
          catch (OperationCanceledException)
          {
            FileSystem.Local.File.Delete(destFileName);

            ProfileSection.Result("Canceled");
            return false;
          }
          catch (Exception ex)
          {
            throw new InvalidOperationException("Downloading failed with exception", ex);
          }
        }
      }
    }

    public static void DownloadFile(string destFileName, Stream responseStream, CancellationToken? token = null, Action<int> indicateProgress = null)
    {
      using (new ProfileSection("Download file", typeof(WebRequestHelper)))
      {
        ProfileSection.Argument("destFileName", destFileName);
        ProfileSection.Argument("responseStream", responseStream);
        ProfileSection.Argument("token", token);
        ProfileSection.Argument("indicateProgress", indicateProgress);

        int bufferSize = Settings.CoreWebDownloadBufferSize.Value;
        using (var fileStream = new FileStream(destFileName, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize))
        {
          var buffer = new byte[bufferSize];
          while (true)
          {
            if (token != null)
            {
              ((CancellationToken)token).ThrowIfCancellationRequested();
            }
            int count = responseStream.Read(buffer, 0, bufferSize);

            if (count == 0)
            {
              break;
            }

            fileStream.Write(buffer, 0, count);
            if (indicateProgress != null)
            {
              indicateProgress(count);
            }
          }
        }
      }
    }

    public static string GetFileName(Uri link, string cookies)
    {
      try
      {
        using (var response = WebRequestHelper.RequestAndGetResponse(link, null, null, cookies))
        {
          return GetFileName(response);
        }
      }
      catch (InvalidOperationException ex)
      {
        Log.Warn("There is a problem with detecting file name of {0}".FormatWith(link), typeof(WebRequestHelper), ex);
        var path = link.AbsolutePath;
        return path.Substring(path.LastIndexOf("/") + 1);
      }
    }

    public static string GetFileName(WebResponse response)
    {
      // example value ==> attachment; filename="Sitecore 6.6.0 rev. 130111.zip"
      var contentDisposition = response.Headers["content-disposition"];
      Assert.IsNotNullOrEmpty(contentDisposition, "The remote server didn't return content-disposition header");

      // it is not just get cookie value but also get some sort of querystirng param
      return GetCookieValue(contentDisposition, "filename").Trim('"');
    }

    public static long GetFileSize(Uri link, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      try
      {
        using (var response = RequestAndGetResponse(link, timeout, readWriteTimeout, cookies))
        {
          return response.ContentLength;
        }
      }
      catch (InvalidOperationException ex)
      {
        Log.Warn("There is a problem with detecting file size of {0}".FormatWith(link), typeof(WebRequestHelper), ex);
        return -1;
      }
    }

    public static string GetCookie(string cookies, string cookieName)
    {
      Assert.ArgumentNotNullOrEmpty(cookies, "cookies");
      Assert.ArgumentNotNullOrEmpty(cookieName, "cookieName");

      return cookies.Split(';').Single(s => s.Split('=')[0].Trim().Equals(cookieName)).Trim();
    }

    public static string GetCookieValue(string cookies, string cookieName)
    {
      Assert.ArgumentNotNullOrEmpty(cookies, "cookies");
      Assert.ArgumentNotNullOrEmpty(cookieName, "cookieName");

      string cookie = GetCookie(cookies, cookieName);
      if (string.IsNullOrEmpty(cookie))
      {
        return null;
      }

      return cookie.Split('=')[1].Trim();
    }

    public static string MakeValidCookie(string authCookie, string session)
    {
      return authCookie + "; " + session;
    }

    public static string GetSessionCookie(string authCookie, string loginUrl)
    {
      var wc = CreateRequest(loginUrl, null, null, authCookie);
      wc.AllowAutoRedirect = false;
      using (var response = wc.GetResponse())
      {
        var cookies = response.Headers[HttpResponseHeader.SetCookie];
        return WebRequestHelper.GetCookie(cookies, "ASP.NET_SessionId");
      }
    }

    public static class Settings
    {
      public static readonly AdvancedProperty<int> CoreWebDownloadTimeoutMinutes = AdvancedSettings.Create("Core/Web/Download/TimeoutMinutes", 120);

      public static readonly AdvancedProperty<int> CoreWebDownloadConnectionTimeout = AdvancedSettings.Create("Core/Web/Connection/Timeout", 100000);

      public static readonly AdvancedProperty<int> CoreWebDownloadBufferSize = AdvancedSettings.Create("Core/Web/Download/BufferSize", 4096);
    }
  }
}

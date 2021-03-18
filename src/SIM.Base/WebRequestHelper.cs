namespace SIM
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using System.Collections.Generic;

  public static class WebRequestHelper
  {
    #region Constants

    public const int Hour = 60 * Minute;
    public const int Minute = 60 * Second;
    public const int Second = 1000;

    #endregion

    #region Public methods

    public static void DownloadFile(string url, string destFileName)
    {
      DownloadFile(new Uri(url), destFileName);
    }

    public static bool DownloadFile(Uri url, string destFileName, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      using (new ProfileSection("Download file"))
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
            FileSystem.FileSystem.Local.File.Delete(destFileName);

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
      using (new ProfileSection("Download file"))
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
            Assert.ArgumentNotNull(responseStream, nameof(responseStream));
            responseStream.ReadTimeout = Settings.CoreWebDownloadTimeoutMinutes.Value * Minute;
            DownloadFile(destFileName, responseStream, token, indicateProgress);

            ProfileSection.Result("Done");
            return true;
          }
          catch (OperationCanceledException)
          {
            FileSystem.FileSystem.Local.File.Delete(destFileName);

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
      using (new ProfileSection("Download file"))
      {
        ProfileSection.Argument("destFileName", destFileName);
        ProfileSection.Argument("responseStream", responseStream);
        ProfileSection.Argument("token", token);
        ProfileSection.Argument("indicateProgress", indicateProgress);

        var bufferSize = Settings.CoreWebDownloadBufferSize.Value;
        using (var fileStream = new FileStream(destFileName, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize))
        {
          var buffer = new byte[bufferSize];
          while (true)
          {
            if (token != null)
            {
              ((CancellationToken)token).ThrowIfCancellationRequested();
            }

            var count = responseStream.Read(buffer, 0, bufferSize);

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

    public static string DownloadString(string url, int? timeout = null, int? readWriteTimeout = null, string cookies = null, Dictionary<string, string> headers = null)
    {
      using (var response = RequestAndGetResponse(url, timeout, readWriteTimeout, cookies: cookies, headers: headers))
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

    public static string GetCookie(string cookies, string cookieName)
    {
      Assert.ArgumentNotNullOrEmpty(cookies, nameof(cookies));
      Assert.ArgumentNotNullOrEmpty(cookieName, nameof(cookieName));

      return cookies.Split(';').Single(s => s.Split('=')[0].Trim().Equals(cookieName)).Trim();
    }

    public static string GetCookieValue(string cookies, string cookieName)
    {
      Assert.ArgumentNotNullOrEmpty(cookies, nameof(cookies));
      Assert.ArgumentNotNullOrEmpty(cookieName, nameof(cookieName));

      var cookie = GetCookie(cookies, cookieName);
      if (string.IsNullOrEmpty(cookie))
      {
        return null;
      }

      return cookie.Split('=')[1].Trim();
    }

    public static string GetFileName(Uri link, string cookies)
    {
      try
      {
        using (var response = RequestAndGetResponse(link, null, null, cookies))
        {
          return GetFileName(response);
        }
      }
      catch (InvalidOperationException ex)
      {
        Log.Warn(ex, $"There is a problem with detecting file name of {link}");
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

    public static HttpWebResponse RequestAndGetResponse(string url, int? timeout = null, int? readWriteTimeout = null, string cookies = null, Dictionary<string, string> headers = null)
    {
      return RequestAndGetResponse(new Uri(url), timeout, readWriteTimeout, cookies, headers);
    }

    public static HttpWebResponse RequestAndGetResponse(Uri uri, int? timeout = null, int? readWriteTimeout = null, string cookies = null, Dictionary<string, string> headers = null)
    {
      var webRequest = CreateRequest(uri, timeout, readWriteTimeout, cookies, headers);

      return webRequest.GetResponse() as HttpWebResponse;
    }

    #endregion

    #region Private methods

    private static HttpWebRequest CreateRequest(string url, int? timeout = null, int? readWriteTimeout = null, string cookies = null)
    {
      return CreateRequest(new Uri(url), timeout, readWriteTimeout, cookies);
    }

    private static HttpWebRequest CreateRequest(Uri url, int? timeout = null, int? readWriteTimeout = null, string cookies = null, Dictionary<string, string> headers = null)
    {
      var webRequest = (HttpWebRequest)WebRequest.Create(url);
      webRequest.Timeout = timeout ?? Settings.CoreWebDownloadConnectionTimeout.Value;
      webRequest.ReadWriteTimeout = readWriteTimeout ?? Settings.CoreWebDownloadTimeoutMinutes.Value * Minute;

      if (cookies != null)
      {
        webRequest.Headers.Add(HttpRequestHeader.Cookie, cookies);
      }

      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
        {
          webRequest.Headers.Add(header.Key, header.Value);
        }
      }

      return webRequest;
    }

    #endregion

    #region Nested type: Settings

    public static class Settings
    {
      #region Fields

      public static readonly AdvancedProperty<int> CoreWebDownloadBufferSize = AdvancedSettings.Create("Core/Web/Download/BufferSize", 4096);
      public static readonly AdvancedProperty<int> CoreWebDownloadConnectionTimeout = AdvancedSettings.Create("Core/Web/Connection/Timeout", 100000);
      public static readonly AdvancedProperty<int> CoreWebDownloadTimeoutMinutes = AdvancedSettings.Create("Core/Web/Download/TimeoutMinutes", 120);

      #endregion
    }

    #endregion
  }
}
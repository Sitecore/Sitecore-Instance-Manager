using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SIM.Base;

namespace SIM.Instances
{
  public static class InstanceHelper
  {
    /// <summary>
    /// Ensures that the instance is responsive.
    /// </summary>
    /// <param name="instance">The Sitecore instance.</param>
    /// <param name="timeout">The timeout of underlying request (milliseconds).</param>
    /// <param name="reason">The optional label that will be shown in Fiddler2 for debugging.</param>
    public static void StartInstance(Instance instance, int? timeout = null, string reason = null)
    {
      Assert.ArgumentNotNull(instance, "instance");

      string url = instance.GetUrl(@"/sitecore/service/keepalive.aspx?ts=" + DateTime.Now.Ticks + "&reason=" + (reason ?? "default"));
      Assert.IsNotNullOrEmpty(url, "url");
      try
      {
        WebRequestHelper.DownloadString(url, timeout);
      }
      catch (WebException ex)
      {
        string text = "There is an issue with requesting '" + url + "'. ";
        var webResponse = ex.Response;
        if (webResponse != null)
        {
          using (var s = webResponse.GetResponseStream())
          {
            if (s != null)
            {
              using (StreamReader streamReader = new StreamReader(s))
              {
                text = streamReader.ReadToEnd();
              }
            }
            else
            {
              text += "No error response stream provided.";
            }
          }
        }
        else
        {
          text += "No error response provided.";
        }

        string text2 = string.Empty;
        try
        {
          text2 = text.Substring(text.IndexOf("<title>") + "<title>".Length);
          text2 = text2.Substring(0, text2.IndexOf("</title>"));
        }
        catch (Exception)
        {
          text2 = text.Substring(0, Math.Min(text.Length, 200));
        }

        throw new WebException("{0} \r\nStatus: {1} \r\n{2}".FormatWith(ex.Message, ex.Status.ToString(), text2), ex);
      }
    }

    public static bool IsInstanceResponsive(Instance instance, string reason = null)
    {
      try
      {
        // 200ms should be more than enough to respond with empty page
        InstanceHelper.StartInstance(instance, 200, reason);
        return true;
      }
      catch (WebException)
      {
        return false;
      }
    }
  }
}

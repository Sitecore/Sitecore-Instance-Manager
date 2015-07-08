using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Base
{
  using System.Windows;

  public static class AuthenticationHelper
  {
    private const int LifetimeSeconds = 300;

    internal static void LoginAsAdmin([NotNull] Instance instance, [NotNull] Window owner, [CanBeNull] string pageUrl = null, [CanBeNull] string browser = null)
    {
      // Generating unique key to authenticate user
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(owner, "owner");

      if (!InstanceHelperEx.PreheatInstance(instance, owner, true))
      {
        return;
      }

      var authKey = GetTempAuthKey();

      // Generating <guid>.aspx page that will one-time security bypasser
      var pageName = authKey + ".aspx";
      var destFileName = Path.Combine(instance.WebRootPath, "sitecore\\shell\\sim-agent", pageName);

      CreateFile(destFileName);
      var async = new Action(() => DeleteFile(destFileName));
      async.BeginInvoke(null, null);
      string url = "/sitecore/shell/sim-agent/" + pageName;
      var userName = AppSettings.AppLoginAsAdminUserName.Value;
      bool isFrontEnd = false;
      bool clipboard = pageUrl == "$(clipboard)";
      if (clipboard)
      {
        pageUrl = string.Empty;
      }

      if (string.IsNullOrEmpty(pageUrl))
      {
        var value = AppSettings.AppLoginAsAdminPageUrl.Value;
        if (!string.IsNullOrEmpty(value) && !value.EqualsIgnoreCase("/sitecore") && !value.EqualsIgnoreCase("sitecore"))
        {
          pageUrl = value;
          if (!value.StartsWith("/sitecore/"))
          {
            isFrontEnd = true;
          }
        }
      }

      var querystring = (String.IsNullOrEmpty(pageUrl) ? String.Empty : "&page=" + pageUrl) + (string.IsNullOrEmpty(userName) || userName.EqualsIgnoreCase("admin") || userName.EqualsIgnoreCase("sitecore\\admin") ? string.Empty : "&user=" + HttpUtility.UrlEncode(userName));
      querystring = querystring.TrimStart('&');
      if (!string.IsNullOrEmpty(querystring))
      {
        querystring = "?" + querystring;
      }

      
      if (clipboard)
      {
        Clipboard.SetText(instance.GetUrl(url + querystring));
        return;
      }

      InstanceHelperEx.BrowseInstance(instance, owner, url + querystring, isFrontEnd, browser);
    }

    private static string GetTempAuthKey()
    {
      var guid = Guid.NewGuid().ToString().Replace("-", String.Empty);
      guid = guid.Substring(1, guid.Length - 2);
      return guid;
    }

    private static void DeleteFile(string destFileName)
    {
      Thread.Sleep(LifetimeSeconds * 1000);
      FileSystem.Local.File.Delete(destFileName);
    }

    const string FileContentsPattern = @"<%@ Page Language=""C#"" %>

<script runat=""server"">
  private static readonly object SyncRoot = new object();
  private static readonly DateTime endDate = DateTime.Parse(""DATETIME_NOW"", System.Globalization.CultureInfo.InvariantCulture);
  private static bool done;
  void Page_Load(object sender, EventArgs e)
  {
    lock (SyncRoot)
    {
      try
      {
        if (!done && DateTime.Now <= endDate)
        {
          var shellUrlPrefix = @""/sitecore/shell"";
          var userName = Request.QueryString[""user""] ?? ""sitecore\\admin"";
          var pageUrl = Request.QueryString[""page""] ?? shellUrlPrefix;
          Sitecore.Security.Authentication.AuthenticationManager.Login(userName);
          Sitecore.Diagnostics.Log.Warn(string.Format(""Bypassing authentication for {0} account"", userName), this);

          string ticket = Sitecore.Web.Authentication.TicketManager.CreateTicket(userName, shellUrlPrefix);
          if (!string.IsNullOrEmpty(ticket))
          {
            System.Web.HttpContext current = System.Web.HttpContext.Current;
            if (current != null)
            {
              System.Web.HttpCookie cookie = new System.Web.HttpCookie(Sitecore.Web.Authentication.TicketManager.CookieName, ticket) {
                HttpOnly = true,
                Expires = DateTime.Now.Add(Sitecore.Configuration.Settings.Authentication.ClientPersistentLoginDuration)
              };

              current.Response.AppendCookie(cookie);
            }
          }

          done = true;
          Response.Redirect(pageUrl);
        }
        else
        {
          Response.Write(""<h1>Wrong authentication ticket: already used or timed out.</h1>"");
        }
      }
      finally
      {
        done = true;
        var pageFile = Server.MapPath(Request.FilePath);
        if (System.IO.File.Exists(pageFile))
        {
          System.IO.File.Delete(pageFile);
        }
      }
    }
  }
  
</script>";

    private static void CreateFile(string destFileName)
    {
      FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(destFileName));
      FileSystem.Local.File.WriteAllText(destFileName, FileContentsPattern.Replace("DATETIME_NOW", DateTime.Now.AddSeconds(LifetimeSeconds).ToString(CultureInfo.InvariantCulture)));
    }
  }
}
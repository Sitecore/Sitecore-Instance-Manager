﻿namespace SIM.Core
{
  using System;
  using System.Globalization;
  using System.IO;
  using JetBrains.Annotations;
  using SIM.FileSystem;
  using SIM.Instances;

  public static class CoreInstanceAuth
  {
    public static string CreateAuthFile(Instance instance, string url = null)
    {
      url = url ?? GenerateAuthUrl();
      var destFileName = Path.Combine(instance.WebRootPath, url.TrimStart('/'));

      CreateFile(destFileName);
      return destFileName;
    }

    [NotNull]
    public static string GenerateAuthUrl()
    {
      // Generating unique key to authenticate user
      var authKey = GetTempAuthKey();

      return "/sitecore/login/sim-agent/login-" + authKey + ".aspx";
    }

    private static void CreateFile(string destFileName)
    {
      FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(destFileName));
      FileSystem.Local.File.WriteAllText(destFileName, FileContentsPattern.Replace("DATETIME_NOW", DateTime.Now.AddSeconds(LifetimeSeconds).ToString(CultureInfo.InvariantCulture)));
    }

    public static string GetTempAuthKey()
    {
      var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
      guid = guid.Substring(1, guid.Length - 2);
      return guid;
    }

    #region Constants

    private const string FileContentsPattern = @"<%@ Page Language=""C#"" %>

<script runat=""server"">
  private static readonly object SyncRoot = new object();
  private static readonly DateTime EndDate = DateTime.Parse(""DATETIME_NOW"", System.Globalization.CultureInfo.InvariantCulture);
  private static bool done;
  void Page_Load(object sender, EventArgs e)
  {
    lock (SyncRoot)
    {
      try
      {
        if (!done && DateTime.Now <= EndDate)
        {
          var shellUrlPrefix = @""/sitecore/shell"";
          var userName = Request.QueryString[""user""] ?? ""sitecore\\admin"";
          var password = Request.QueryString[""password""] ?? ""b"";

          if (!Sitecore.Security.Accounts.User.Exists(userName))
          {
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
              var user = Sitecore.Security.Accounts.User.Create(userName, password);

              var profile = user.Profile;
              profile.IsAdministrator = true;
              profile.Save();

              Sitecore.Diagnostics.Log.Audit(""Create admin account: "" + userName, this);
            }
          }          
          
          var pageUrl = Request.QueryString[""page""] ?? shellUrlPrefix;
          Sitecore.Security.Authentication.AuthenticationManager.Login(userName);
          Sitecore.Diagnostics.Log.Warn(string.Format(""Bypassing authentication for {0} account"", userName), this);

          var ticket = Sitecore.Web.Authentication.TicketManager.CreateTicket(userName, shellUrlPrefix);
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
    public const int LifetimeSeconds = 300;

    #endregion
  }
}

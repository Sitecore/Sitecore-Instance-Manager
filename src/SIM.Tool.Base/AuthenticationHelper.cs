namespace SIM.Tool.Base
{
  using System;
  using System.Threading;
  using System.Web;
  using System.Windows;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  public static class AuthenticationHelper
  {
    internal static void LoginAsAdmin([NotNull] Instance instance, [NotNull] Window owner, [CanBeNull] string pageUrl = null, [CanBeNull] string browser = null, [CanBeNull] string[] parameters = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(owner, "owner");

      if (!InstanceHelperEx.PreheatInstance(instance, owner, true))
      {
        return;
      }

      // Generating unique url to authenticate user
      var url = CoreInstanceAuth.GenerateAuthUrl();
      var destFileName = CoreInstanceAuth.CreateAuthFile(instance, url);

      // Schedule deletion of the file
      var async = new Action(() => DeleteFile(destFileName));
      async.BeginInvoke(null, null);

      var userName = CoreAppSettings.AppLoginAsAdminUserName.Value;
      var isFrontEnd = false;
      var clipboard = pageUrl == "$(clipboard)";
      if (clipboard)
      {
        pageUrl = string.Empty;
      }

      if (string.IsNullOrEmpty(pageUrl))
      {
        var value = CoreAppSettings.AppLoginAsAdminPageUrl.Value;
        if (!string.IsNullOrEmpty(value) && !value.EqualsIgnoreCase("/sitecore") && !value.EqualsIgnoreCase("sitecore"))
        {
          pageUrl = value;
          if (!value.StartsWith("/sitecore/"))
          {
            isFrontEnd = true;
          }
        }
      }

      var querystring = (string.IsNullOrEmpty(pageUrl) ? string.Empty : "&page=" + pageUrl) + (string.IsNullOrEmpty(userName) || userName.EqualsIgnoreCase("admin") || userName.EqualsIgnoreCase("sitecore\\admin") ? string.Empty : "&user=" + HttpUtility.UrlEncode(userName));
      querystring = querystring.TrimStart('&');
      if (!string.IsNullOrEmpty(querystring))
      {
        querystring = "?" + querystring;
      }

      if (clipboard)
      {
        Clipboard.SetDataObject(instance.GetUrl(url + querystring));
      }
      else
      {
        InstanceHelperEx.BrowseInstance(instance, owner, url + querystring, isFrontEnd, browser, parameters);
      }
    }

    private static void DeleteFile(string destFileName)
    {
      Thread.Sleep(CoreInstanceAuth.LifetimeSeconds * 1000);
      FileSystem.FileSystem.Local.File.Delete(destFileName);
    }
  }
}
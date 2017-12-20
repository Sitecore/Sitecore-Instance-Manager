namespace SIM.Adapters.WebServer
{
  using System;
  using System.Linq;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public static class WebServerManager
  {
    #region Public Methods

    [NotNull]
    public static WebServerContext CreateContext()
    {
      return new WebServerContext();
    }

    public static void DeleteWebsite([NotNull] long id)
    {
      Log.Info($"Deleting website {id}");

      using (WebServerContext context = CreateContext())
      {
        Site site = context.Sites.SingleOrDefault(s => s.Id == id);
        if (site != null)
        {
          DeleteWebsite(context, site);
        }
      }
    }                              

    [NotNull]
    public static string GetWebRootPath([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, nameof(site));

      Application apps = site.Applications["/"];
      if (apps != null)
      {
        VirtualDirectory vdirs = apps.VirtualDirectories["/"];
        if (vdirs != null)
        {
          ConfigurationAttribute phpath = vdirs.Attributes["physicalPath"];
          if (phpath != null)
          {
            var value = (string)phpath.Value;
            if (!string.IsNullOrEmpty(value))
            {
              return value;
            }
          }
        }
      }

      throw new Exception($"IIS website {site.Id} seems to be corrupted or misconfigured");
    }

    public static bool HostBindingExists([NotNull] string host)
    {
      Assert.ArgumentNotNull(host, nameof(host));

      bool result;
      using (WebServerContext context = CreateContext())
      {
        result = context.Sites.Any(site => site.Bindings.Any(binding => binding.Host.EqualsIgnoreCase(host)));
      }

      return result;
    }

    public static bool AddHostBinding([NotNull] string siteName, [NotNull] BindingInfo binding)
    {
      Assert.ArgumentNotNull(siteName, nameof(siteName));
      Assert.ArgumentNotNull(binding, nameof(binding));

      using (WebServerContext context = CreateContext())
      {
        Site siteInfo = context.Sites.FirstOrDefault(site => site.Name.EqualsIgnoreCase(siteName));
        if (HostBindingExists(binding.Host) || siteInfo == null)
        {
          return false;
        }
        var bindingInformation = $"{binding.IP}:{binding.Port}:{binding.Host}";
        
        siteInfo.Bindings.Add(bindingInformation, binding.Protocol);
        context.CommitChanges();
      }

      return true;
    }               

    public static bool WebsiteExists([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      bool v;
      using (WebServerContext context = CreateContext())
      {
        v = context.Sites.Any(s => s.Name.EqualsIgnoreCase(name));
      }

      return v;
    }

    #endregion

    #region Methods

    private static void DeleteWebsite([NotNull] WebServerContext context, [NotNull] Site site)
    {
      Assert.ArgumentNotNull(context, nameof(context));
      Assert.ArgumentNotNull(site, nameof(site));

      foreach (Application application in site.Applications)
      {
        var applicationPoolName = application.ApplicationPoolName;
        ApplicationPool appplicationPool = context.ApplicationPools[applicationPoolName];

        // Application is used only in the current website or isn't used at all
        if (appplicationPool != null && context.Sites.Count(s => s.ApplicationDefaults.ApplicationPoolName.EqualsIgnoreCase(applicationPoolName)) <= 1)
        {
          context.ApplicationPools.Remove(appplicationPool);
        }
      }

      context.Sites.Remove(site);
      context.CommitChanges();
    }

    #endregion
    
    public sealed class WebServerContext : IDisposable
    {
      [NotNull]
      private ServerManager ServerManager { get; } = new ServerManager();
      
      [NotNull]
      public ApplicationPoolCollection ApplicationPools => ServerManager.ApplicationPools.IsNotNull();

      [NotNull]
      public SiteCollection Sites => ServerManager.Sites.IsNotNull();
      
      public void CommitChanges()
      {
        ServerManager.CommitChanges();
      }
      
      public void Dispose()
      {
        ServerManager.Dispose();
      }
      
    }
  }
}
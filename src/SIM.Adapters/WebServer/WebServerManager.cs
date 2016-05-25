namespace SIM.Adapters.WebServer
{
  using System;
  using System.Linq;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class WebServerManager
  {
    #region Public Methods

    [NotNull]
    public static WebServerContext CreateContext([CanBeNull] string debugLocation, object callingClass = null)
    {
      return new WebServerContext(debugLocation, callingClass);
    }

    public static void DeleteWebsite([NotNull] long id)
    {
      Log.Info("Deleting website {0}", id);

      using (WebServerContext context = CreateContext("WebServerManager.DeleteWebsite({0})".FormatWith(id)))
      {
        Site site = context.Sites.SingleOrDefault(s => s.Id == id);
        if (site != null)
        {
          DeleteWebsite(context, site);
        }
      }
    }

    public static void DeleteWebsite([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      Log.Info("Deleting website {0}", name);
      using (WebServerContext context = CreateContext("WebServerManager.DeleteWebsite('{0}')".FormatWith(name)))
      {
        Site site = context.Sites[name];
        if (site != null)
        {
          DeleteWebsite(context, site);
        }
      }
    }

    [NotNull]
    public static string GetWebRootPath([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");

      Application apps = site.Applications["/"];
      if (apps != null)
      {
        VirtualDirectory vdirs = apps.VirtualDirectories["/"];
        if (vdirs != null)
        {
          ConfigurationAttribute phpath = vdirs.Attributes["physicalPath"];
          if (phpath != null)
          {
            string value = (string)phpath.Value;
            if (!string.IsNullOrEmpty(value))
            {
              return value;
            }
          }
        }
      }

      throw new Exception("IIS website " + site.Id + " seems to be corrupted or misconfigured");
    }

    public static bool HostBindingExists([NotNull] string host)
    {
      Assert.ArgumentNotNull(host, "host");

      bool result;
      using (WebServerContext context = CreateContext("WebServerManager.HostBindingExists('{0}')".FormatWith(host)))
      {
        result = context.Sites.Any(site => site.Bindings.Any(binding => binding.Host.EqualsIgnoreCase(host)));
      }

      return result;
    }

    public static bool AddHostBinding([NotNull] string siteName, [NotNull] BindingInfo binding)
    {
      Assert.ArgumentNotNull(siteName, "siteName");
      Assert.ArgumentNotNull(binding, "binding");

      using (WebServerContext context = CreateContext("WebServerManager.AddHostBinding('{0}','{1}')".FormatWith(siteName, binding.Host)))
      {
        Site siteInfo = context.Sites.FirstOrDefault(site => site.Name.EqualsIgnoreCase(siteName));
        if (HostBindingExists(binding.Host) || siteInfo == null)
        {
          return false;
        }
        string bindingInformation = binding.IP + ":" + binding.Port + ":" + binding.Host;
        
        siteInfo.Bindings.Add(bindingInformation, binding.Protocol);
        context.CommitChanges();
      }

      return true;
    }

    public static bool IsApplicationPoolRunning([NotNull] ApplicationPool appPool)
    {
      Assert.ArgumentNotNull(appPool, "appPool");

      return appPool.WorkerProcesses.Count > 0 && appPool.WorkerProcesses.Any(wp => wp != null && wp.State == WorkerProcessState.Running);
    }

    public static bool WebsiteExists([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      bool v;
      using (WebServerContext context = CreateContext("WebServerManager.WebsiteExists('{0}')".FormatWith(name)))
      {
        v = context.Sites.Any(s => s.Name.EqualsIgnoreCase(name));
      }

      return v;
    }

    #endregion

    #region Methods

    private static void DeleteWebsite([NotNull] WebServerContext context, [NotNull] Site site)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(site, "site");

      foreach (Application application in site.Applications)
      {
        string applicationPoolName = application.ApplicationPoolName;
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

    #region Nested type: WebServerContext

    public sealed class WebServerContext : ProfileSection
    {
      #region Fields

      public string DebugLocation;
      private readonly ServerManager serverManager = new ServerManager();

      #endregion

      #region Constructors

      public WebServerContext(string debugLocation, object caller = null) : base("{0} (IIS)".FormatWith(debugLocation), caller)
      {
      }

      #endregion

      #region Properties

      [NotNull]
      public ApplicationPoolCollection ApplicationPools
      {
        get
        {
          return this.serverManager.ApplicationPools;
        }
      }

      [NotNull]
      public SiteCollection Sites
      {
        get
        {
          return this.serverManager.Sites;
        }
      }

      #endregion

      #region Public Methods

      public void CommitChanges()
      {
        this.serverManager.CommitChanges();
      }

      #endregion

      #region Implemented Interfaces

      #region IDisposable

      public override void Dispose()
      {
        this.serverManager.Dispose();
        base.Dispose();
      }

      #endregion

      #endregion
    }

    #endregion
  }
}
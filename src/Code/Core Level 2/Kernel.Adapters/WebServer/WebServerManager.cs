#region Usings

using System;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Base;

#endregion

namespace SIM.Adapters.WebServer
{
  #region

  

  #endregion

  /// <summary>
  ///   The web server manager.
  /// </summary>
  public static class WebServerManager
  {
    #region Public Methods

    /// <summary>
    /// The create context.
    /// </summary>
    /// <param name="debugLocation">
    /// The debug Location. 
    /// </param>
    /// <exception cref="Exception">
    /// Can't get exclusive access to IIS Manager
    /// </exception>
    [NotNull]
    public static WebServerContext CreateContext([CanBeNull] string debugLocation, object callingClass = null)
    {
      return new WebServerContext(debugLocation, callingClass);
    }

    /// <summary>
    /// The delete website.
    /// </summary>
    /// <param name="id">
    /// The id. 
    /// </param>
    public static void DeleteWebsite([NotNull] long id)
    {
      SIM.Base.Log.Info("Deleting website {0}".FormatWith(id), typeof(WebServerManager));

      using (WebServerContext context = CreateContext("WebServerManager.DeleteWebsite({0})".FormatWith(id)))
      {
        Site site = context.Sites.SingleOrDefault(s => s.Id == id);
        if (site != null)
        {
          DeleteWebsite(context, site);
        }
      }
    }

    /// <summary>
    /// The delete website.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    public static void DeleteWebsite([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      SIM.Base.Log.Info("Deleting website {0}".FormatWith(name), typeof(WebServerManager));
      using (WebServerContext context = CreateContext("WebServerManager.DeleteWebsite('{0}')".FormatWith(name)))
      {
        Site site = context.Sites[name];
        if (site != null)
        {
          DeleteWebsite(context, site);
        }
      }
    }

    /// <summary>
    /// The get web root path.
    /// </summary>
    /// <param name="site">
    /// The site. 
    /// </param>
    /// <exception cref="Exception">
    /// <c>Exception</c>
    ///   .
    /// </exception>
    /// <returns>
    /// The get web root path. 
    /// </returns>
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

    /// <summary>
    /// The host binding exists.
    /// </summary>
    /// <param name="host">
    /// The host. 
    /// </param>
    /// <returns>
    /// The host binding exists. 
    /// </returns>
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

    /// <summary>
    /// The is application pool running.
    /// </summary>
    /// <param name="appPool">
    /// The app pool. 
    /// </param>
    /// <returns>
    /// The is application pool running. 
    /// </returns>
    public static bool IsApplicationPoolRunning([NotNull] ApplicationPool appPool)
    {
      Assert.ArgumentNotNull(appPool, "appPool");

      return appPool.WorkerProcesses.Count > 0 && appPool.WorkerProcesses.Any(wp => wp != null && wp.State == WorkerProcessState.Running);
    }

    /// <summary>
    /// The website exists.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <returns>
    /// The website exists. 
    /// </returns>
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

    /// <summary>
    /// The delete website.
    /// </summary>
    /// <param name="context">
    /// The context. 
    /// </param>
    /// <param name="site">
    /// The site. 
    /// </param>
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

    /// <summary>
    ///   The web server context.
    /// </summary>
    public sealed class WebServerContext : ProfileSection
    {
      #region Fields

      /// <summary>
      ///   The server manager.
      /// </summary>
      private readonly ServerManager serverManager = new ServerManager();

      /// <summary>
      ///   The debug location.
      /// </summary>
      public string DebugLocation;

      #endregion

      #region Constructors

      /// <summary>
      ///   Initializes a new instance of the <see cref="WebServerContext" /> class.
      /// </summary>
      public WebServerContext(string debugLocation, object caller = null) : base("{0} (IIS)".FormatWith(debugLocation), caller)
      {
      }

      #endregion

      #region Properties

      /// <summary>
      ///   Gets ApplicationPools.
      /// </summary>
      [NotNull]
      public ApplicationPoolCollection ApplicationPools
      {
        get
        {
          return this.serverManager.ApplicationPools;
        }
      }

      /// <summary>
      ///   Gets Sites.
      /// </summary>
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

      /// <summary>
      ///   The commit changes.
      /// </summary>
      public void CommitChanges()
      {
        this.serverManager.CommitChanges();
      }

      #endregion

      #region Implemented Interfaces

      #region IDisposable

      /// <summary>
      ///   The dispose.
      /// </summary>
      public override void Dispose()
      {
        serverManager.Dispose();
        base.Dispose();
      }

      #endregion

      #endregion
    }

    #endregion
  }
}
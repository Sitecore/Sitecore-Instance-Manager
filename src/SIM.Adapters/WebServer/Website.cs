namespace SIM.Adapters.WebServer
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public class Website
  {
    #region Fields

    public readonly long ID;

    #endregion

    #region Constructors

    public Website(long id)
    {
      this.ID = id;
    }

    protected Website()
    {
    }

    #endregion

    #region Properties

    #region Public properties

    public virtual ObjectState ApplicationPoolState
    {
      get
      {
        ObjectState result;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.ApplicationPoolState"))
        {
          ApplicationPool pool = this.GetPool(context);
          result = pool.State;
        }

        return result;
      }
    }

    [NotNull]
    public virtual IEnumerable<BindingInfo> Bindings
    {
      get
      {
        var list = new List<BindingInfo>();
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Get website bindings", this))
        {
          Site site = this.GetSite(context);

          var bindings = site.Bindings;
          Assert.IsNotNull(bindings, "bindings");

          foreach (Binding binding in bindings.Where(x => x.Protocol.StartsWith("http", StringComparison.OrdinalIgnoreCase)))
          {
            try
            {
              list.Add(new BindingInfo(binding));
            }
            catch (Exception ex)
            {
              Log.Error(ex, "Cannot retrieve binding info");
            }
          }
        }

        return list;
      }
    }

    [NotNull]
    public virtual IEnumerable<string> HostNames
    {
      get
      {
        List<string> list = new List<string>();
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Get website hostnames"))
        {
          Site site = this.GetSite(context);
          {
            foreach (Binding binding in site.Bindings)
            {
              string host = binding.Host;
              if (string.IsNullOrEmpty(host))
              {
                host = "*";
              }

              list.Add(host);
            }
          }
        }

        return list;
      }
    }

    public virtual bool Is32Bit
    {
      get
      {
        bool result;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Is32Bit"))
        {
          ApplicationPool pool = this.GetPool(context);
          result = pool.Enable32BitAppOnWin64;
        }

        return result;
      }
    }

    public virtual bool IsClassic
    {
      get
      {
        bool result;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.IsClassic"))
        {
          ApplicationPool pool = this.GetPool(context);
          result = pool.ManagedPipelineMode == ManagedPipelineMode.Classic;
        }

        return result;
      }
    }

    public virtual bool IsNetFramework4
    {
      get
      {
        bool result;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.IsNetFramework4"))
        {
          ApplicationPool pool = this.GetPool(context);
          result = pool.GetAttribute("managedRuntimeVersion").Value as string == "v4.0";
        }

        return result;
      }
    }

    [NotNull]
    public virtual string Name
    {
      get
      {
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).Name".FormatWith(this.ID)))
        {
          return this.GetName(context);
        }
      }
    }

    public virtual IEnumerable<int> ProcessIds
    {
      get
      {
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.ProcessIds"))
        {
          ApplicationPool pool = this.GetPool(context);
          return pool.WorkerProcesses.NotNull().Select(process => process.ProcessId);
        }
      }
    }

    [NotNull]
    public virtual string WebRootPath
    {
      get
      {
        string w;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).WebRootPath".FormatWith(this.ID)))
        {
          Site site = this.GetSite(context);
          Assert.IsNotNull(site, "The '{0}' site can't be found".FormatWith(this.ID));

          w = WebServerManager.GetWebRootPath(site);
        }

        return w;
      }
    }

    public bool IsDisabled
    {
      get
      {
        return this.Name.ToLowerInvariant().EndsWith("_disabled");
      }

      set
      {
        var name = this.Name.TrimEnd("_disabled");
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).Name".FormatWith(this.ID)))
        {
          context.Sites[name].Name = name + "_disabled";
          context.CommitChanges();
        }
      }
    }

    #endregion

    #region Public methods

    public virtual string GetName(WebServerManager.WebServerContext context)
    {
      return this.GetSite(context).Name;
    }

    [NotNull]
    public virtual XmlDocumentEx GetWebConfig(string webRootPath = null)
    {
      XmlDocumentEx xmlDocumentEx;
      using (new ProfileSection("Get web.config", this))
      {
        ProfileSection.Argument("webRootPath", webRootPath);

        xmlDocumentEx = WebServer.WebConfig.GetWebConfig(webRootPath ?? this.WebRootPath);

        return ProfileSection.Result(xmlDocumentEx);
      }
    }

    #endregion

    #endregion

    #region public virtual Methods

    [NotNull]
    public virtual string GetUrl([CanBeNull] string path = null)
    {
      var binding = this.Bindings.FirstOrDefault();
      Assert.IsNotNull(binding, "Website " + this.ID + " has no url bindings");
      var url = binding.Protocol + "://";
      var host = binding.Host;
      url += host != "*" ? host : Environment.MachineName;
      var port = binding.Port;
      if (port != 80)
      {
        url += ":" + port;
      }

      if (!string.IsNullOrEmpty(path))
      {
        url += '/' + path.TrimStart('/');
      }

      return url;
    }

    public virtual void Recycle()
    {
      Log.Info("Recycle the {0} instance's application pool", this.Name);

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Recycle"))
      {
        ApplicationPool pool = this.GetPool(context);
        if (this.IsStarted(pool))
        {
          pool.Recycle();
          context.CommitChanges();
        }
      }
    }

    public virtual void Start()
    {
      Log.Info("Starting website {0}", this.ID);
      
      if (IsDisabled)
      {
        throw new InvalidOperationException("The {0} website is disabled. Open IIS Manager and remove _disabled suffix from its name in order to enable the website.");
      }

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Start.Pool"))
      {
        Site site = this.GetSite(context);
        Assert.IsNotNull(site, "Site is missing");
        ApplicationPool pool = this.GetPool(context);
        Assert.IsNotNull(pool, "pool");
        if (!this.IsStarted(pool))
        {
          pool.Start();
          context.CommitChanges();
        }
      }

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Start.Site"))
      {
        Site site = this.GetSite(context);
        Assert.IsNotNull(site, "Site is missing");
        if (!IsStarted(site))
        {
          site.Start();
          context.CommitChanges();
        }
      }
    }

    public virtual void Stop(bool? force = null)
    {
      Log.Info("Stop website {0} ({1})", this.Name, this.ID);

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Stop"))
      {
        ApplicationPool pool = this.GetPool(context);

        if (force ?? false)
        {
          foreach (WorkerProcess workerProcess in pool.WorkerProcesses)
          {
            try
            {
              Process process = Process.GetProcessById(workerProcess.ProcessId);
              process.Kill();
            }
            catch (Exception ex)
            {
              Log.Warn(ex, "Stop website {0} ({1}) failed", this.Name, this.ID);
            }
          }
        }

        if (this.IsStarted(pool))
        {
          pool.Stop();
          context.CommitChanges();
        }
      }
    }

    public virtual void StopApplicationPool()
    {
      Log.Info("Stop app pool {0} ({1})", this.Name, this.ID);

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.StopApplicationPool"))
      {
        ApplicationPool pool = this.GetPool(context);
        if (this.IsStarted(pool))
        {
          pool.Stop();
          context.CommitChanges();
        }
      }
    }

    #endregion

    #region Methods

    [NotNull]
    public virtual ApplicationPool GetPool([NotNull] WebServerManager.WebServerContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      var site = this.GetSite(context);
      var application = site.Applications.FirstOrDefault(ap => ap.Path.EqualsIgnoreCase("/"));
      Assert.IsNotNull(application, "Cannot find root application for {0} site".FormatWith(site.Name));
      string poolname = application.ApplicationPoolName;
      ApplicationPool pool = context.ApplicationPools[poolname];
      Assert.IsNotNull(pool, "The " + poolname + "application pool doesn't exists");

      return pool;
    }

    [NotNull]
    public virtual Site GetSite([NotNull] WebServerManager.WebServerContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      Site site = context.Sites.SingleOrDefault(s => s.Id == this.ID);
      Assert.IsNotNull(site, "Website " + this.ID + " not found");
      return site;
    }

    public virtual void SetAppPoolMode(bool? is40 = null, bool? is32 = null)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.SetAppPoolMode"))
      {
        ApplicationPool pool = this.GetPool(context);
        if (is32 != null)
        {
          pool.Enable32BitAppOnWin64 = (bool)is32;
        }

        if (is40 != null)
        {
          pool.SetAttributeValue("managedRuntimeVersion", (bool)is40 ? "v4.0" : "v2.0");
        }

        context.CommitChanges();
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return "ID: {0}, {1}".FormatWith(this.ID, this.Name);
    }

    #endregion

    #region Private methods

    private bool IsStarted([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");

      return site.State == ObjectState.Started || site.State == ObjectState.Starting;
    }

    private bool IsStarted([NotNull] ApplicationPool pool)
    {
      Assert.ArgumentNotNull(pool, "pool");

      return pool.State == ObjectState.Started || pool.State == ObjectState.Starting;
    }

    #endregion
  }
}
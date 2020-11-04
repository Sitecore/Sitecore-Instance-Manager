namespace SIM.Adapters.WebServer
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public class Website
  {
    #region Fields

    public long ID { get; }

    private string name;

    #endregion

    #region Constructors

    public Website(long id)
    {
      ID = id;
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          ApplicationPool pool = GetPool(context);
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          Site site = GetSite(context);

          var bindings = site.Bindings;
          Assert.IsNotNull(bindings, nameof(bindings));

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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          Site site = GetSite(context);
          {
            foreach (Binding binding in site.Bindings)
            {
              var host = binding.Host;
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          ApplicationPool pool = GetPool(context);
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          ApplicationPool pool = GetPool(context);
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          ApplicationPool pool = GetPool(context);
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
        try
        {
          if (string.IsNullOrEmpty(name))
          {
            using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
            {
              name = GetName(context);
            }
          }
          return name;
        }
        catch(Exception ex)
        {
          Log.Error(ex, ex.Message);
          return "Error";
        }
      }
    }

    public virtual IEnumerable<int> ProcessIds
    {
      get
      {
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          ApplicationPool pool = GetPool(context);
          return Extensions.NotNull(pool.WorkerProcesses).Select(process => process.ProcessId);
        }
      }
    }

    [NotNull]
    public virtual string WebRootPath
    {
      get
      {
        string w;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
        {
          Site site = GetSite(context);
          Assert.IsNotNull(site, $"The '{ID}' site can't be found");

          w = WebServerManager.GetWebRootPath(site);
        }

        return w;
      }
    }

    public bool IsDisabled
    {
      get
      {
        return Name.ToLowerInvariant().EndsWith("_disabled");
      }    
    }

    #endregion

    #region Public methods

    public virtual string GetName(WebServerManager.WebServerContext context)
    {
      return GetSite(context).Name;
    }

    [NotNull]
    public virtual XmlDocumentEx GetWebConfig(string webRootPath = null)
    {
      XmlDocumentEx xmlDocumentEx;
      using (new ProfileSection("Get web.config", this))
      {
        ProfileSection.Argument("webRootPath", webRootPath);

        xmlDocumentEx = WebServer.WebConfig.GetWebConfig(webRootPath ?? WebRootPath);

        return ProfileSection.Result(xmlDocumentEx);
      }
    }

    #endregion

    #endregion

    #region public virtual Methods

    [NotNull]
    public virtual string GetUrl([CanBeNull] string path = null)
    {
      var binding = Bindings
        .OrderBy(x => string.Equals(x.Protocol, "https", StringComparison.OrdinalIgnoreCase) ? 0 : 1) // to use https when available
        .FirstOrDefault();
      Assert.IsNotNull(binding, $"Website {ID} has no url bindings");
      var url = binding.Protocol + "://";
      var host = binding.Host;
      url += host != "*" ? host : Environment.MachineName;
      var port = binding.Port;
      if (port != 80)
      {
        url += $":{port}";
      }

      if (!string.IsNullOrEmpty(path))
      {
        url += '/' + path.TrimStart('/');
      }

      return url;
    }

    public virtual void Recycle()
    {
      Log.Info($"Recycle the {Name} instance's application pool");

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        ApplicationPool pool = GetPool(context);
        if (IsStarted(pool))
        {
          pool.Recycle();
          context.CommitChanges();
        }
      }
    }

    public virtual void Start()
    {
      Log.Info($"Starting website {ID}");
      
      if (IsDisabled)
      {
        throw new InvalidOperationException("The {0} website is disabled. Open IIS Manager and remove _disabled suffix from its name in order to enable the website.");
      }

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        Site site = GetSite(context);
        Assert.IsNotNull(site, "Site is missing");
        ApplicationPool pool = GetPool(context);
        Assert.IsNotNull(pool, nameof(pool));
        if (!IsStarted(pool))
        {
          pool.Start();
          context.CommitChanges();
        }
      }

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        Site site = GetSite(context);
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
      Log.Info($"Stop website {Name} ({ID})");

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        ApplicationPool pool = GetPool(context);

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
              Log.Warn(ex, $"Stop website {Name} ({ID}) failed");
            }
          }
        }

        if (IsStarted(pool))
        {
          pool.Stop();
          context.CommitChanges();
        }
      }
    }

    public virtual void StopApplicationPool()
    {
      Log.Info($"Stop app pool {Name} ({ID})");

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        ApplicationPool pool = GetPool(context);
        if (IsStarted(pool))
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
      Assert.ArgumentNotNull(context, nameof(context));

      var site = GetSite(context);
      var application = site.Applications.FirstOrDefault(ap => ap.Path.EqualsIgnoreCase("/"));
      Assert.IsNotNull(application, $"Cannot find root application for {site.Name} site");
      var poolname = application.ApplicationPoolName;
      ApplicationPool pool = context.ApplicationPools[poolname];
      Assert.IsNotNull(pool, $"The {poolname}application pool doesn\'t exists");

      return pool;
    }

    [NotNull]
    public virtual Site GetSite([NotNull] WebServerManager.WebServerContext context)
    {
      Assert.ArgumentNotNull(context, nameof(context));

      Site site = context.Sites.SingleOrDefault(s => s.Id == ID);
      Assert.IsNotNull(site, $"Website {ID} not found");
      return site;
    }

    public virtual void SetAppPoolMode(bool? is40 = null, bool? is32 = null)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        ApplicationPool pool = GetPool(context);
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
      return $"ID: {ID}, {Name}";
    }

    #endregion

    #region Private methods

    private bool IsStarted([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, nameof(site));

      return site.State == ObjectState.Started || site.State == ObjectState.Starting;
    }

    private bool IsStarted([NotNull] ApplicationPool pool)
    {
      Assert.ArgumentNotNull(pool, nameof(pool));

      return pool.State == ObjectState.Started || pool.State == ObjectState.Starting;
    }

    #endregion
  }
}
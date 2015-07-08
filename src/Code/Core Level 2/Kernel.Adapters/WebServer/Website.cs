#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Base;

#endregion

namespace SIM.Adapters.WebServer
{
  #region



  #endregion

  /// <summary>
  ///   The website.
  /// </summary>
  public class Website
  {
    #region Fields

    /// <summary>
    ///   The id.
    /// </summary>
    public readonly long ID;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Website"/> class.
    /// </summary>
    /// <param name="id">
    /// The id. 
    /// </param>
    public Website(long id)
    {
      this.ID = id;
    }

    protected Website() { }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets HostNames.
    /// </summary>
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

    /// <summary>
    ///   Gets Bindings.
    /// </summary>
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
              Log.Error("Cannot retrieve binding info", this, ex);
            }
          }
        }

        return list;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether Is32Bit.
    /// </summary>
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

    /// <summary>
    ///   Gets a value indicating whether IsClassic.
    /// </summary>
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

    /// <summary>
    ///   Gets a value indicating whether IsNetFramework4.
    /// </summary>
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

    /// <summary>
    ///   Gets Name.
    /// </summary>
    [NotNull]
    public virtual string Name
    {
      get
      {
        string name;
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).Name".FormatWith(ID)))
        {
          return GetName(context);
        }
      }
    }

    public virtual string GetName(WebServerManager.WebServerContext context)
    {
      return this.GetSite(context).Name;
    }

    /// <summary>
    ///   Gets WebRootPath.
    /// </summary>
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

    /// <summary>
    ///   Gets GetWebConfig.
    /// </summary>
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

    #region public virtual Methods

    /// <summary>
    /// The get url.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <returns>
    /// The get url. 
    /// </returns>
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

    /// <summary>
    ///   The start.
    /// </summary>
    public virtual void Start()
    {
      SIM.Base.Log.Info("Starting website {0}".FormatWith(this.ID), this);

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Start"))
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

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Start"))
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

    /// <summary>
    /// The stop.
    /// </summary>
    /// <param name="force">
    /// The force. 
    /// </param>
    public virtual void Stop(bool force = false)
    {
      SIM.Base.Log.Info("Stop website {0} ({1})".FormatWith(this.Name, this.ID), this);

      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website.Stop"))
      {
        ApplicationPool pool = this.GetPool(context);

        if (force)
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
              SIM.Base.Log.Warn("Stop website {0} ({1}) failed".FormatWith(this.Name, this.ID), this, ex);
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
      SIM.Base.Log.Info("Stop app pool {0} ({1})".FormatWith(this.Name, this.ID), this);

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

    public virtual void Recycle()
    {
      Log.Info("Recycle the {0} instance's application pool".FormatWith(this.Name), this);

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

    #endregion

    #region Methods

    /// <summary>
    /// The get pool.
    /// </summary>
    /// <param name="context">
    /// The context. 
    /// </param>
    /// <returns>
    /// </returns>
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

    /// <summary>
    /// The get site.
    /// </summary>
    /// <param name="context">
    /// The context. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public virtual Site GetSite([NotNull] WebServerManager.WebServerContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      Site site = context.Sites.SingleOrDefault(s => s.Id == this.ID);
      Assert.IsNotNull(site, "Website " + this.ID + " not found");
      return site;
    }

    #endregion

    public override string ToString()
    {
      return "ID: {0}, {1}".FormatWith(this.ID, this.Name);
    }

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
  }
}
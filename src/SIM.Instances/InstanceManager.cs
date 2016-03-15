namespace SIM.Instances
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Web.Administration;
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class InstanceManager
  {
    #region Fields

    private static IEnumerable<Instance> cachedInstances;
    private static IEnumerable<Instance> instances;
    private static string instancesFolder;

    #endregion

    #region Properties

    #region Delegates

    public static event EventHandler InstancesListUpdated;

    #endregion

    #region Public properties

    [CanBeNull]
    public static IEnumerable<Instance> Instances
    {
      get
      {
        return instances;
      }

      private set
      {
        instances = value;
        OnInstancesListUpdated();
      }
    }

    [CanBeNull]
    public static IEnumerable<Instance> PartiallyCachedInstances
    {
      get
      {
        // I believe that this check is redundant because the this list is filled before Instances list is actually filled.
        if (cachedInstances != null)
        {
          return cachedInstances;
        }

        if (Instances == null)
        {
          return null;
        }

        cachedInstances = Instances.Select(x => new PartiallyCachedInstance((int)x.ID)).ToArray();
        return cachedInstances;
      }

      private set
      {
        if (cachedInstances != null)
        {
          foreach (var cachedInstance in cachedInstances.OfType<IDisposable>())
          {
            if (cachedInstance != null)
            {
              cachedInstance.Dispose();
            }
          }
        }

        cachedInstances = value;
      }
    }

    #endregion

    #endregion

    #region Public Methods

    #region Public methods

    [CanBeNull]
    public static Instance GetInstance([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");
      Log.Debug("InstanceManager:GetInstance('{0}')", name);

      Initialize();
      if (Instances == null)
      {
        return null;
      }

      return Instances.SingleOrDefault(i => i.Name.EqualsIgnoreCase(name));
    }

    public static void Initialize([CanBeNull] string defaultRootFolder = null)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize instance manager"))
      {
        ProfileSection.Argument("defaultRootFolder", defaultRootFolder);

        IEnumerable<Site> sites = GetOperableSites(context, defaultRootFolder);
        PartiallyCachedInstances = GetPartiallyCachedInstances(sites);
        Instances = GetInstances();
      }
    }
    
    public static void InitializeWithSoftListRefresh([CanBeNull] string defaultRootFolder = null)
    {
      using (new ProfileSection("Initialize with soft list refresh"))
      {
        // Add check that this isn't an initial initialization
        if (Instances == null)
        {
          Initialize(defaultRootFolder);
        }

        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize with soft list refresh"))
        {
          IEnumerable<Site> sites = GetOperableSites(context, defaultRootFolder);

          // The trick is in reused PartiallyCachedInstances. We use site ID as identificator that cached instance may be reused. If we can't fetch instance from cache, we create new.
          PartiallyCachedInstances = sites
            .Select(site => PartiallyCachedInstances.FirstOrDefault(cachedInst => cachedInst.ID == site.Id) ?? GetPartiallyCachedInstance(site))
            .Where(IsSitecore)
            .ToArray();

          Instances = PartiallyCachedInstances.Select(x => GetInstance(x.ID)).ToArray();
        }
      }
    }

    #endregion

    #region Private methods

    private static IEnumerable<Instance> GetInstances()
    {
      using (new ProfileSection("Get instances"))
      {
        var array = PartiallyCachedInstances.Select(x => GetInstance(x.ID)).ToArray();

        return ProfileSection.Result(array);
      }
    }

    private static IEnumerable<Instance> GetPartiallyCachedInstances(IEnumerable<Site> sites)
    {
      using (new ProfileSection("Getting partially cached Sitecore instances"))
      {
        ProfileSection.Argument("sites", sites);

        var array = sites.Select(GetPartiallyCachedInstance).Where(IsSitecore).ToArray();

        return ProfileSection.Result(array);
      }
    }

    #endregion

    #endregion

    #region Methods

    [NotNull]
    private static Instance GetInstance([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");

      int id = (Int32)site.Id;
      Log.Debug("InstanceManager:GetInstance(Site: {0})", site.Id);
      return new Instance(id);
    }

    private static IEnumerable<Site> GetOperableSites([NotNull] WebServerManager.WebServerContext context, [CanBeNull] string defaultRootFolder = null)
    {
      Assert.IsNotNull(context, "Context cannot be null");

      using (new ProfileSection("Getting operable sites"))
      {
        ProfileSection.Argument("context", context);
        ProfileSection.Argument("defaultRootFolder", defaultRootFolder);

        IEnumerable<Site> sites = context.Sites;
        if (defaultRootFolder != null)
        {
          instancesFolder = defaultRootFolder.ToLower();
          sites = sites.Where(s => WebServerManager.GetWebRootPath(s).ToLower().Contains(instancesFolder));
        }

        return ProfileSection.Result(sites);
      }
    }

    [NotNull]
    private static Instance GetPartiallyCachedInstance([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");
      int id = (Int32)site.Id;
      Log.Debug("InstanceManager:GetPartiallyCachedInstance(Site: {0})", site.Id);
      return new PartiallyCachedInstance(id);
    }

    private static bool IsSitecore([CanBeNull] Instance instance)
    {
      return instance != null && instance.IsSitecore;
    }

    private static void OnInstancesListUpdated()
    {
      EventHandler handler = InstancesListUpdated;
      if (handler != null)
      {
        handler(null, EventArgs.Empty);
      }
    }

    #endregion

    #region Public methods

    public static Instance GetInstance(long id)
    {
      using (new ProfileSection("Get instance by id"))
      {
        ProfileSection.Argument("id", id);

        var instance = new Instance((int)id);

        return ProfileSection.Result(instance);
      }
    }

    #endregion
  }
}
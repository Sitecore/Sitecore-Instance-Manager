#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Adapters.WebServer;
using SIM.Base;

#endregion

namespace SIM.Instances
{
  #region



  #endregion

  /// <summary>
  ///   The instance manager.
  /// </summary>
  public static class InstanceManager
  {
    #region Fields

    /// <summary>
    ///   The instances folder.
    /// </summary>
    private static string instancesFolder;

    /// <summary>
    /// Backing field for instances collection
    /// </summary>
    private static IEnumerable<Instance> instances;

    private static IEnumerable<Instance> cachedInstances;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Instances.
    /// </summary>
    [CanBeNull]
    public static IEnumerable<Instance> Instances
    {
      get { return instances; }
      private set
      {
        instances = value;
        OnInstancesListUpdated();
      }
    }

    /// <summary>
    /// Gets instances with particular properties caching to improve the performance. If you need actual values, use Instances property instead.
    /// The following properties are cached: Name, WebRootPath, LicensePath, ProductFullName.
    /// </summary>
    [CanBeNull]
    public static IEnumerable<Instance> PartiallyCachedInstances
    {
      get
      {
        //I believe that this check is redundant because the this list is filled before Instances list is actually filled.
        if (cachedInstances != null)
          return cachedInstances;
        if (Instances == null)
          return null;
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



    /// <summary>
    /// This event is fired if instances list was updated.
    /// </summary>
    public static event EventHandler InstancesListUpdated;

    #endregion

    #region Public Methods

    [CanBeNull]
    public static Instance GetInstance([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");
      Log.Debug("InstanceManager:GetInstance('{0}')".FormatWith(name));

      Initialize();
      if (Instances == null)
      {
        return null;
      }

      return Instances.SingleOrDefault(i => i.Name.EqualsIgnoreCase(name));
    }

    /// <summary>
    /// Entirely invalidates all the caches and reinitializes Instance Manager
    /// </summary>
    /// <param name="defaultRootFolder">Show only instances located beneath of this folder. Null means show all of them.</param>
    public static void Initialize([CanBeNull] string defaultRootFolder = null)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize instance manager", typeof(InstanceManager)))
      {
        ProfileSection.Argument("defaultRootFolder", defaultRootFolder);

        IEnumerable<Site> sites = GetOperableSites(context, defaultRootFolder, string.IsNullOrEmpty(defaultRootFolder));
        PartiallyCachedInstances = GetPartiallyCachedInstances(sites);
        Instances = GetInstances();
      }
    }

    /// <summary>
    /// Entirely invalidates all the caches and reinitializes Instance Manager
    /// </summary>
    /// <param name="defaultRootFolder"></param>
    /// <param name="detectEverywhere"></param>
    [Obsolete]
    public static void Initialize([CanBeNull] string defaultRootFolder, bool? detectEverywhere)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize instance manager", typeof(InstanceManager)))
      {
        ProfileSection.Argument("defaultRootFolder", defaultRootFolder);
        ProfileSection.Argument("detectEverywhere", detectEverywhere);

        IEnumerable<Site> sites = GetOperableSites(context, defaultRootFolder, detectEverywhere);
        PartiallyCachedInstances = GetPartiallyCachedInstances(sites);
        Instances = GetInstances();
      }
    }

    private static IEnumerable<Instance> GetInstances()
    {
      using (new ProfileSection("Get instances", typeof(InstanceManager)))
      {
        var array = PartiallyCachedInstances.Select(x => GetInstance(x.ID)).ToArray();

        return ProfileSection.Result(array);
      }
    }

    private static IEnumerable<Instance> GetPartiallyCachedInstances(IEnumerable<Site> sites)
    {
      using (new ProfileSection("Getting partially cached Sitecore instances", typeof(InstanceManager)))
      {
        ProfileSection.Argument("sites", sites);

        var array = sites.Select(GetPartiallyCachedInstance).Where(IsSitecore).ToArray();

        return ProfileSection.Result(array);
      }
    }

    /// <summary>
    /// Reinitializes Instance Manager with partial cache invalidating. It should be used when we do not expect foreign changes in IIS. 
    /// </summary>
    /// <param name="defaultRootFolder"></param>
    /// <param name="detectEverywhere"></param>
    public static void InitializeWithSoftListRefresh([CanBeNull] string defaultRootFolder = null, bool? detectEverywhere = null)
    {
      using (new ProfileSection("Initialize with soft list refresh", typeof(InstanceManager)))
      {
        //Add check that this isn't an initial initialization
        if (Instances == null)
        {
          Initialize(defaultRootFolder, detectEverywhere);
        }

        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize with soft list refresh", typeof(InstanceManager)))
        {
          IEnumerable<Site> sites = GetOperableSites(context, defaultRootFolder, detectEverywhere);

          //The trick is in reused PartiallyCachedInstances. We use site ID as identificator that cached instance may be reused. If we can't fetch instance from cache, we create new.
          PartiallyCachedInstances = sites
            .Select(site => PartiallyCachedInstances.FirstOrDefault(cachedInst => cachedInst.ID == site.Id) ?? GetPartiallyCachedInstance(site))
            .Where(IsSitecore)
            .ToArray();

          Instances = PartiallyCachedInstances.Select(x => GetInstance(x.ID)).ToArray();
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Mehtod is used to fetch sites from IIS.
    /// </summary>
    /// <param name="context">WebServerContext to work in</param>
    /// <param name="defaultRootFolder"></param>
    /// <param name="detectEverywhere"></param>
    /// <returns></returns>
    private static IEnumerable<Site> GetOperableSites([NotNull]WebServerManager.WebServerContext context, [CanBeNull] string defaultRootFolder = null, bool? detectEverywhere = null)
    {
      Assert.IsNotNull(context, "Context cannot be null");

      using (new ProfileSection("Getting operable sites", typeof(InstanceManager)))
      {
        ProfileSection.Argument("context", context);
        ProfileSection.Argument("defaultRootFolder", defaultRootFolder);
        ProfileSection.Argument("detectEverywhere", detectEverywhere);

        if (defaultRootFolder != null)
        {
          instancesFolder = defaultRootFolder.ToLower();
        }

        IEnumerable<Site> sites = context.Sites;
        var detectEverywhereResult = detectEverywhere ?? Settings.CoreInstancesDetectEverywhere.Value;
        if (!detectEverywhereResult)
        {
          if (string.IsNullOrEmpty(instancesFolder))
          {
            SIM.Base.Log.Warn("Since the 'Detect.Instances.Everywhere' setting is disabled and the instances root isn't set in the Settings dialog, the 'C:\\inetpub\\wwwroot' will be used instead", typeof(InstanceManager));

            instancesFolder = @"C:\inetpub\wwwroot";
          }
          instancesFolder = instancesFolder.ToLower();
          sites = sites.Where(s => WebServerManager.GetWebRootPath(s).ToLower().Contains(instancesFolder));
        }

        return ProfileSection.Result(sites);
      }

    }

    [NotNull]
    private static Instance GetInstance([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");

      int id = (Int32)site.Id;
      Log.Debug("InstanceManager:GetInstance(Site: {0})".FormatWith(site.Id));
      return new Instance(id);
    }

    [NotNull]
    private static Instance GetPartiallyCachedInstance([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");
      int id = (Int32)site.Id;
      Log.Debug("InstanceManager:GetPartiallyCachedInstance(Site: {0})".FormatWith(site.Id));
      return new PartiallyCachedInstance(id);
    }

    private static bool IsSitecore([CanBeNull] Instance instance)
    {
      return instance != null && instance.IsSitecore;
    }

    /// <summary>
    /// Event invokator for instances list 
    /// </summary>
    private static void OnInstancesListUpdated()
    {
      EventHandler handler = InstancesListUpdated;
      if (handler != null) handler(null, EventArgs.Empty);
    }

    #endregion

    public static Instance GetInstance(long id)
    {
      using (new ProfileSection("Get instance by id", typeof(InstanceManager)))
      {
        ProfileSection.Argument("id", id);

        var instance = new Instance((int) id);

        return ProfileSection.Result(instance);
      }
    }

    public static class Settings
    {
      public readonly static AdvancedProperty<bool> CoreInstancesDetectEverywhere = AdvancedSettings.Create("Core/Instances/DetectEverywhere", false);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Text;
using SIM.Adapters.SqlServer;
using SIM.Products;

namespace SIM.Instances
{
  using System.IO;
  using System.Xml;
  using SIM.Base;

  /// <summary>
  /// Instance with cached property values. Because of performance some property values are cached and may return non actual values.
  /// The following properties are cached: Name, WebRootPath, LicensePath, ProductFullName.
  /// </summary>
  public sealed class PartiallyCachedInstance : Instance, IDisposable
  {
    #region Instance fields

    [CanBeNull]
    private readonly FileSystemWatcher webConfigWatcher;

    [CanBeNull]
    private readonly FileSystemWatcher appConfigWatcher;

    private string licencePath;
    private string name;
    private string productFullName;
    private string webRootPath;

    [CanBeNull]
    private XmlDocument webConfigResultCache;

    [CanBeNull]
    private string modulesNamesCache;

    #endregion

    #region Constructors

    public PartiallyCachedInstance(int id) : base(id)
    {
      var path = this.WebRootPath;
      if(!File.Exists(path))
      {
        return;  
      }
      
      var webConfig = new FileSystemWatcher(path, "web.config");
      this.webConfigWatcher = webConfig;
      webConfig.IncludeSubdirectories = false;
      webConfig.Changed += this.ClearCache;
      webConfig.EnableRaisingEvents = true;
      var appConfigPath = Path.Combine(path, "App_Config");
      if(!Directory.Exists(appConfigPath))
      {
        return;
      }
      
      var appConfig = new FileSystemWatcher(appConfigPath, "*.config");
      this.appConfigWatcher = appConfig;
      appConfig.IncludeSubdirectories = true;
      appConfig.Changed += this.ClearCache;
      appConfig.EnableRaisingEvents = true;
    }

    private void ClearCache([CanBeNull] object sender, [CanBeNull] FileSystemEventArgs fileSystemEventArgs)
    {
      this.webConfigResultCache = null;
      this.modulesNamesCache = null;
    }

    public PartiallyCachedInstance(Instance instance) : this((int) instance.ID)
    {
    }

    #endregion

    #region Public Properties

    public override string LicencePath
    {
      get { return licencePath ?? (licencePath = base.LicencePath); }
    }

    public override string ProductFullName
    {
      get { return productFullName ?? (productFullName = base.ProductFullName); }
    }

    public override string Name
    {
      get { return name ?? (name = base.Name); }
    }

    public override string WebRootPath
    {
      get { return webRootPath ?? (webRootPath = base.WebRootPath); }
    }

    #endregion

    public override XmlDocument GetWebResultConfig(bool normalize = false)
    {
      return this.webConfigResultCache ?? (this.webConfigResultCache = base.GetWebResultConfig(normalize));
    }

    public override string ModulesNames
    {
      get
      {
        return this.modulesNamesCache ?? (this.modulesNamesCache = base.ModulesNames);
      }
    }

    public void Dispose()
    {
      if(this.appConfigWatcher != null)
      {
        this.appConfigWatcher.EnableRaisingEvents = false;
      }
      
      if(this.webConfigWatcher != null)
      {
        this.webConfigWatcher.EnableRaisingEvents = false;
      }
    }
  }
}
namespace SIM.Instances
{
  using System;
  using System.IO;
  using System.Xml;
  using JetBrains.Annotations;

  public sealed class PartiallyCachedInstance : Instance, IDisposable
  {
    #region Instance fields

    [CanBeNull]
    private FileSystemWatcher AppConfigWatcher { get; }

    [CanBeNull]
    private FileSystemWatcher WebConfigWatcher { get; }

    private string _LicencePath;

    [CanBeNull]
    private string _ModulesNamesCache;

    private string _Name;
    private string _ProductFullName;

    [CanBeNull]
    private XmlDocument _WebConfigResultCache;

    private string _WebRootPath;
    private string _BindingsNames;

    #endregion

    #region Constructors

    public PartiallyCachedInstance(int id) : base(id)
    {
      var path = this.WebRootPath;
      if (!File.Exists(path))
      {
        return;
      }

      var webConfig = new FileSystemWatcher(path, "web.config");
      this.WebConfigWatcher = webConfig;
      webConfig.IncludeSubdirectories = false;
      webConfig.Changed += this.ClearCache;
      webConfig.EnableRaisingEvents = true;
      var appConfigPath = Path.Combine(path, "App_Config");
      if (!Directory.Exists(appConfigPath))
      {
        return;
      }

      var appConfig = new FileSystemWatcher(appConfigPath, "*.config");
      this.AppConfigWatcher = appConfig;
      appConfig.IncludeSubdirectories = true;
      appConfig.Changed += this.ClearCache;
      appConfig.EnableRaisingEvents = true;
    }

    public PartiallyCachedInstance(Instance instance) : this((int)instance.ID)
    {
    }

    #endregion

    #region Public Properties

    public override string LicencePath
    {
      get
      {
        return this._LicencePath ?? (this._LicencePath = base.LicencePath);
      }
    }

    public override string Name
    {
      get
      {
        return this._Name ?? (this._Name = base.Name);
      }
    }

    public override string ProductFullName
    {
      get
      {
        return this._ProductFullName ?? (this._ProductFullName = base.ProductFullName);
      }
    }

    public override string WebRootPath
    {
      get
      {
        return this._WebRootPath ?? (this._WebRootPath = base.WebRootPath);
      }
    }

    #endregion

    #region Public properties

    public override string ModulesNames
    {
      get
      {
        return this._ModulesNamesCache ?? (this._ModulesNamesCache = base.ModulesNames);
      }
    }

    public override string BindingsNames
    {
      get
      {
        return this._BindingsNames ?? (this._BindingsNames = base.BindingsNames);
      }
    }

    #endregion

    #region Public methods

    public void Dispose()
    {
      if (this.AppConfigWatcher != null)
      {
        this.AppConfigWatcher.EnableRaisingEvents = false;
      }

      if (this.WebConfigWatcher != null)
      {
        this.WebConfigWatcher.EnableRaisingEvents = false;
      }
    }

    public override XmlDocument GetWebResultConfig(bool normalize = false)
    {
      return this._WebConfigResultCache ?? (this._WebConfigResultCache = base.GetWebResultConfig(normalize));
    }

    #endregion

    #region Private methods

    private void ClearCache([CanBeNull] object sender, [CanBeNull] FileSystemEventArgs fileSystemEventArgs)
    {
      this._WebConfigResultCache = null;
      this._ModulesNamesCache = null;
    }

    #endregion
  }
}
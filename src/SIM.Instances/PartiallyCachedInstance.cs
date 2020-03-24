using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SIM.Instances
{
  using System;
  using System.IO;
  using System.Xml;
  using JetBrains.Annotations;

  public sealed class PartiallyCachedInstance : Instance, IDisposable, INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Constructors

    public PartiallyCachedInstance(int id) : base(id)
    {
      var path = WebRootPath;
      if (!File.Exists(path))
      {
        return;
      }

      var webConfig = new FileSystemWatcher(path, "web.config");
      WebConfigWatcher = webConfig;
      webConfig.IncludeSubdirectories = false;
      webConfig.Changed += ClearCache;
      webConfig.EnableRaisingEvents = true;
      var appConfigPath = Path.Combine(path, "App_Config");
      if (!Directory.Exists(appConfigPath))
      {
        return;
      }

      var appConfig = new FileSystemWatcher(appConfigPath, "*.config");
      AppConfigWatcher = appConfig;
      appConfig.IncludeSubdirectories = true;
      appConfig.Changed += ClearCache;
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
        return _LicencePath ?? (_LicencePath = base.LicencePath);
      }
    }

    public override string Name
    {
      get
      {
        return _Name ?? (_Name = base.Name);
      }
    }

    public override string ProductFullName
    {
      get
      {
        return _ProductFullName ?? (_ProductFullName = base.ProductFullName);
      }
    }

    public override string WebRootPath
    {
      get
      {
        return _WebRootPath ?? (_WebRootPath = base.WebRootPath);
      }
    }

    #endregion

    #region Public properties

    public override string ModulesNames
    {
      get
      {
        if (_ModulesNamesCache == null)
        {
          Task.Run(() => { this._ModulesNamesCache = base.ModulesNames; OnPropertyChanged(); });
          return string.Empty;
        }

        return _ModulesNamesCache;
      }
    }

    public override string BindingsNames
    {
      get
      {
        return _BindingsNames ?? (_BindingsNames = base.BindingsNames);
      }
    }

    #endregion

    #region Public methods

    public void Dispose()
    {
      if (AppConfigWatcher != null)
      {
        AppConfigWatcher.EnableRaisingEvents = false;
      }

      if (WebConfigWatcher != null)
      {
        WebConfigWatcher.EnableRaisingEvents = false;
      }
    }

    public override XmlDocument GetWebResultConfig(bool normalize = false)
    {
      return _WebConfigResultCache ?? (_WebConfigResultCache = base.GetWebResultConfig(normalize));
    }

    #endregion

    #region Private methods

    private void ClearCache([CanBeNull] object sender, [CanBeNull] FileSystemEventArgs fileSystemEventArgs)
    {
      _WebConfigResultCache = null;
      _ModulesNamesCache = null;
    }

    #endregion

    #region Protected methods
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

  }
}
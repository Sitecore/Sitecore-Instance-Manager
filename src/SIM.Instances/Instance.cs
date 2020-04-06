namespace SIM.Instances
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using Microsoft.Web.Administration;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances.RuntimeSettings;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.SitecoreEnvironments;

  [Serializable]
  public class Instance : Website, IXmlSerializable
  {                                                              
    #region Instance fields

    protected RuntimeSettingsAccessor _RuntimeSettingsAccessor;
    private InstanceConfiguration _Configuration;

    private Product _Product;

    #endregion

    #region Constructors

    public Instance([NotNull] int id)
      : base(id)
    {
    }


    protected Instance()
    {
    }

    #endregion

    #region Non-public Properties

    [NotNull]
    protected virtual RuntimeSettingsAccessor RuntimeSettingsAccessor
    {
      get
      {
        return _RuntimeSettingsAccessor ??
               (_RuntimeSettingsAccessor = RuntimeSettingsManager.GetRealtimeSettingsAccessor(this));
      }
    }

    #endregion

    #region Public Properties

    #region Public properties

    public virtual ICollection<Database> AttachedDatabases
    {
      get
      {
        return GetAttachedDatabases();
      }
    }

    public virtual IEnumerable<InstanceBackup> Backups
    {
      get
      {
        return GetBackups();
      }
    }

    public virtual string BackupsFolder
    {
      get
      {
        return GetBackupsFolder();
      }
    }

    [NotNull]
    public virtual InstanceConfiguration Configuration
    {
      get
      {
        return _Configuration ?? (_Configuration = new InstanceConfiguration(this));
      }
    }          

    public virtual string DataFolderPath
    {
      get
      {
        return GetDataFolderPath();
      }
    }

    [CanBeNull]
    public virtual string IndexesFolderPath
    {
      get
      {
        try
        {
          var indexFolder = RuntimeSettingsAccessor.GetSitecoreSettingValue("IndexFolder");
          Assert.IsNotNull(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> value is empty string");
          return MapPath(indexFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(DataFolderPath, "Indexes");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get indexes folder of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get indexes folder of {WebRootPath}");
        }
      }
    }

    public virtual bool IsSitecore
    {
      get
      {
        try
        {
          return FileSystem.FileSystem.Local.File.Exists(ProductHelper.GetKernelPath(WebRootPath));
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred during checking if it is Sitecore");

          return false;
        }
      }
    }

    public virtual bool IsSitecoreEnvironmentMember
    {
      get
      {
        try
        {
          if (!IsSitecore)
          {
            foreach (SitecoreEnvironment sitecoreEnvironment in SitecoreEnvironmentHelper.SitecoreEnvironments)
            {
              foreach (SitecoreEnvironmentMember sitecoreEnvironmentMember in sitecoreEnvironment.Members)
              {
                if (sitecoreEnvironmentMember.Name == Name)
                {
                  return true;
                }
              }
            }
          }

          return false;
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred during checking if it is Sitecore environment member");

          return false;
        }
      }
    }

    public virtual bool IsHidden
    {
      get
      {
        return GetIsHidden();
      }
    }

    [CanBeNull]
    public virtual string LicencePath
    {
      get
      {
        try
        {
          var licenseFile = RuntimeSettingsAccessor.GetSitecoreSettingValue("LicenseFile");
          Assert.IsNotNull(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> value is empty string");
          return MapPath(licenseFile);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(DataFolderPath, "license.xml");
          if (FileSystem.FileSystem.Local.File.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get license file of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get license file of {WebRootPath}");
        }
      }
    }

    [NotNull]
    public virtual string LogsFolderPath
    {
      get
      {
        return GetLogsFolderPath();
      }
    }

    public ICollection<MongoDbDatabase> MongoDatabases
    {
      get
      {
        return GetMongoDatabases();
      }
    }

    [CanBeNull]
    public virtual string PackagesFolderPath
    {
      get
      {
        try
        {
          var packagePath = RuntimeSettingsAccessor.GetSitecoreSettingValue("PackagePath");
          Assert.IsNotNull(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> value is empty string");
          return MapPath(packagePath);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(DataFolderPath, "Packages");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get packages folder of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get packages folder of {WebRootPath}");
        }
      }
    }


    [NotNull]
    public virtual SitecoreEnvironment SitecoreEnvironment
    {
      get { return SitecoreEnvironmentHelper.GetExistingOrNewSitecoreEnvironment(this.Name); }
    }

    [NotNull]
    public virtual Product Product
    {
      get
      {
        return _Product ?? (_Product = ProductManager.GetProduct(ProductFullName));
      }
    }

    [NotNull]
    public virtual string ProductFullName
    {
      get
      {
        return ProductHelper.DetectProductFullName(WebRootPath);
      }
    }

    public virtual string RootPath
    {
      get
      {
        return GetRootPath();
      }
    }

    [CanBeNull]
    public virtual string SerializationFolderPath
    {
      get
      {
        try
        {
          var serializationFolder = RuntimeSettingsAccessor.GetSitecoreSettingValue("SerializationFolder");
          Assert.IsNotNull(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> value is empty string");
          return MapPath(serializationFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(DataFolderPath, "Serialization");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get serialization folder of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get serialization folder of {WebRootPath}");
        }
      }
    }

    public InstanceState State
    {
      get
      {
        if (IsDisabled)
        {
          return InstanceState.Disabled;
        }

        if (ApplicationPoolState == ObjectState.Stopped || ApplicationPoolState == ObjectState.Stopping)
        {
          return InstanceState.Stopped;
        }

        if (ProcessIds.Any())
        {
          return InstanceState.Running;
        }

        return InstanceState.Ready;
      }
    }

    public virtual string TempFolderPath
    {
      get
      {
        return GetTempFolderPath();
      }
    }

    #endregion

    #region Private methods

    private ICollection<MongoDbDatabase> GetMongoDatabases()
    {
      using (new ProfileSection("Get mongo databases", this))
      {
        return RuntimeSettingsAccessor.GetMongoDatabases();
      }
    }

    #endregion

    #endregion

    #region IXmlSerializable Members

    public virtual XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public virtual void ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    public virtual void WriteXml(XmlWriter writer)
    {
      foreach (var property in GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml($"<Instance>{xml.OuterXml}</Instance>")), false);
          continue;
        }

        writer.WriteElementString(property.Name, string.Empty, (value ?? string.Empty).ToString());
      }

      ;
    }

    #endregion

    #region Public Methods

    #region Public properties

    public virtual string DisplayName
    {
      get
      {
        return $"{base.ToString()} ({ProductFullName})";
      }
    }

    [UsedImplicitly]
    public virtual string ModulesNames
    {
      get
      {
        try
        {
          var omsVersions = new[] {"6.2", "6.3", "6.4"};
          var dmsVersions = new[] {"6.5", "6.6", "7.0", "7.1", "7.2"};
          var dmsName = omsVersions.Any(x => ProductFullName.Contains(x))
            ? "OMS"
            : (dmsVersions.Any(x => ProductFullName.Contains(x)) ? "DMS" : "xDB");

          var modulesNames = Modules.Select(x => x.Name.TrimStart("Sitecore "));
          return (string.Join(", ", modulesNames) +
                  (File.Exists(Path.Combine(WebRootPath, "App_Config\\Include\\Sitecore.Analytics.config"))
                    ? $", {dmsName}"
                    : string.Empty)).TrimStart(" ,".ToCharArray());
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"Issue with reading ModulesNames propery of {this.Name} instance.");
          return string.Empty;
        }
      }
    }

    [UsedImplicitly]
    public virtual string BindingsNames
    {
      get
      {
        return
          $"Hosts: {string.Join(", ", Bindings.Select(x => (x.Host.EmptyToNull() ?? x.IP) + (x.Port != 80 ? $":{x.Port}" : "")))}";
      }
    }

    [NotNull]
    public virtual IReadOnlyCollection<Product> Modules
    {
      get
      {
        return ModulesFiles.Select(x => ProductManager.GetProduct(x)).Where(x => x != Product.Undefined).ToArray();
      }
    }

    [NotNull]
    public virtual IReadOnlyCollection<FileInfo> ModulesFiles
    {
      get
      {
        var dir = new DirectoryInfo(PackagesFolderPath);
        
        return dir.Exists ? dir.GetFiles("*.zip") : new FileInfo[0];
      }
    }

    #endregion

    #region Public methods

    [NotNull]
    public virtual string GetBackupFolder(string name)
    {
      var backups = GetBackupsFolder();
      return Path.Combine(backups, name);
    }

    [NotNull]
    public virtual XmlDocument GetShowconfig(bool normalize = false)
    {
      using (new ProfileSection("Get showconfig.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return RuntimeSettingsAccessor.GetShowconfig(normalize);
      }
    }

    [CanBeNull]
    public virtual IEnumerable<string> GetVisualStudioSolutionFiles(string searchPattern = null)
    {
      var rootPath = RootPath;
      var webRootPath = WebRootPath;
      return VisualStudioHelper.GetVisualStudioSolutionFiles(rootPath, webRootPath, searchPattern).ToArray();
    }

    [NotNull]
    public virtual XmlDocument GetWebResultConfig(bool normalize = false)
    {
      using (new ProfileSection("Get web.config.result.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return RuntimeSettingsAccessor.GetWebConfigResult(normalize);
      }
    }

    public override string ToString()
    {
      return DisplayName;
    }

    #endregion

    #endregion

    #region Non-public Methods

    #region Protected methods

    [NotNull]
    protected virtual ICollection<Database> GetAttachedDatabases()
    {
      using (new ProfileSection("Get attached databases", this))
      {
        return RuntimeSettingsAccessor.GetDatabases();
      }
    }

    [CanBeNull]
    protected virtual IEnumerable<InstanceBackup> GetBackups()
    {
      var root = this.SafeCall(GetBackupsFolder);
      if (string.IsNullOrEmpty(root) || !FileSystem.FileSystem.Local.Directory.Exists(root))
      {
        yield break;
      }

      var directories = FileSystem.FileSystem.Local.Directory.GetDirectories(root)
        .Select(x => new DirectoryInfo(x))
        .OrderBy(x => x.CreationTimeUtc)
        .ToArray();

      foreach (var childInfo in directories)
      {
        var date = string.Format("\"{2}\", {0}, {1:hh:mm:ss tt}", childInfo.CreationTime.ToString("yyyy-MM-dd"), 
          childInfo.CreationTime, childInfo.Name);

        if (string.IsNullOrEmpty(date))
        {
          continue;
        }

        var backup = new InstanceBackup(date, childInfo.FullName);
        if (!FileSystem.FileSystem.Local.Directory.Exists(backup.DatabasesFolderPath) && !FileSystem.FileSystem.Local.Directory.Exists(backup.MongoDatabasesFolderPath) && !FileSystem.FileSystem.Local.File.Exists(backup.WebRootFilePath) &&
            !FileSystem.FileSystem.Local.File.Exists(backup.WebRootNoClientFilePath))
        {
          continue;
        }

        yield return backup;
      }
    }

    [NotNull]
    protected virtual string GetBackupsFolder()
    {
      var rootPath = GetRootPath();
      if (WebRootPath.EqualsIgnoreCase(rootPath))
      {
        DirectoryInfo parent = new DirectoryInfo(rootPath).Parent;
        Assert.IsNotNull(parent, "Instance isn't permitted to use drive root as web root path");
        return Path.Combine(parent.FullName, Name + " backups");
      }

      return Path.Combine(rootPath, "Backups");
    }

    [NotNull]
    protected virtual string GetDataFolderPath()
    {
      using (new ProfileSection("Get data folder path", this))
      {
        try
        {
          var dataFolder = RuntimeSettingsAccessor.GetScVariableValue("dataFolder");
          Assert.IsNotNull(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element value is empty string");

          return MapPath(dataFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(Path.GetDirectoryName(WebRootPath), "Data");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get data folder of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get data folder of {WebRootPath}");
        }
      }
    }

    protected virtual bool GetIsHidden()
    {
      try
      {
        return FileSystem.FileSystem.Local.File.Exists(System.IO.Path.Combine(WebRootPath, "hidden.txt"));
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during checking if it is hidden");

        return false;
      }
    }

    protected virtual string GetLogsFolderPath()
    {
      using (new ProfileSection("Get log folder path", this))
      {
        try
        {
          var dataFolder = RuntimeSettingsAccessor.GetSitecoreSettingValue("LogFolder");
          var result = MapPath(dataFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var dataLogs = Path.Combine(DataFolderPath, "logs");
          if (FileSystem.FileSystem.Local.Directory.Exists(dataLogs))
          {
            Log.Error(ex, $"Cannot get logs folder of {WebRootPath}");

            return dataLogs;
          }

          throw new InvalidOperationException($"Cannot get logs folder of {WebRootPath}");
        }
      }
    }

    protected virtual string GetRootFolderViaDatabases(ICollection<Database> databases)
    {
      var webRootPath = WebRootPath;
      using (new ProfileSection("Get root folder (using databases)", this))
      {
        ProfileSection.Argument("databases", databases);

        foreach (var database in databases)
        {
          Log.Debug($"Database: {database}");
          var fileName = database.FileName;
          if (string.IsNullOrEmpty(fileName))
          {
            Log.Warn($"The {database.RealName} database seems to be detached since it doesn't have a FileName property filled in");
            continue;
          }

          Log.Debug($"name: {database.Name}, fileName: {fileName}");
          var folder = Path.GetDirectoryName(fileName);
          if (folder.ContainsIgnoreCase(webRootPath))
          {
            continue;
          }

          Assert.IsNotNullOrEmpty(folder, nameof(folder));
          var common = FileSystem.FileSystem.Local.Directory.FindCommonParent(webRootPath, folder);
          if (string.IsNullOrEmpty(common))
          {
            continue;
          }

          if (Math.Abs(FileSystem.FileSystem.Local.Directory.GetDistance(webRootPath, common)) <= 1)
          {
            return ProfileSection.Result(common);
          }
        }

        return ProfileSection.Result((string)null);
      }
    }

    [NotNull]
    protected virtual string GetRootPath()
    {
      using (new ProfileSection("Get instance's root path", this))
      {
        try
        {
          var webRootPath = WebRootPath;
          var dataFolderPath = GetDataFolderPath();
          Assert.IsNotNullOrEmpty(dataFolderPath, nameof(dataFolderPath));

          // data folder is inside website folder
          if (dataFolderPath.ContainsIgnoreCase(WebRootPath))
          {
            // find using only databases
            var result = GetRootFolderViaDatabases(GetAttachedDatabases()) ?? webRootPath;

            return ProfileSection.Result(result);
          }


          // trying to detect using databases
          var common = GetRootFolderViaDatabases(GetAttachedDatabases());
          if (common != null)
          {
            return ProfileSection.Result(common);
          }

          // trying to detect via data folder
          var detectedRoot = FileSystem.FileSystem.Local.Directory.FindCommonParent(webRootPath, dataFolderPath);

          // if impossible
          if (string.IsNullOrEmpty(detectedRoot))
          {
            return ProfileSection.Result(webRootPath);
          }

          InvalidConfigurationException.Assert(!webRootPath.ContainsIgnoreCase(dataFolderPath), 
            "The data folder accidentally was set to be parent ({0}) of the website root folder ({1})".FormatWith(
              dataFolderPath, webRootPath));
          var distance = FileSystem.FileSystem.Local.Directory.GetDistance(webRootPath, detectedRoot);
          InvalidConfigurationException.Assert(distance <= 1, 
            "Cannot detect the Root Folder - the detection result ({1}) is too far from the Website ({0}) folder"
              .FormatWith(WebRootPath, detectedRoot));

          return ProfileSection.Result(detectedRoot);
        }
        catch (Exception ex)
        {
          var rootData = Path.GetDirectoryName(WebRootPath);
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, $"Cannot get root folder of {WebRootPath}");

            return rootData;
          }

          throw new InvalidOperationException($"Cannot get root folder of {WebRootPath}");
        }
      }
    }

    [NotNull]
    protected virtual string GetTempFolderPath()
    {
      using (new ProfileSection("Get temp folder path", this))
      {
        try
        {
          var tempFolder = RuntimeSettingsAccessor.GetScVariableValue("tempFolder");
          Assert.IsNotNull(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element value is empty string");

          var result = MapPath(tempFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var websiteTemp = Path.Combine(WebRootPath, "temp");
          if (FileSystem.FileSystem.Local.Directory.Exists(websiteTemp))
          {
            Log.Error(ex, $"Cannot get temp folder of {WebRootPath}");

            return websiteTemp;
          }

          throw new InvalidOperationException($"Cannot get temp folder of {WebRootPath}");
        }
      }
    }

    protected virtual string MapPath(string virtualPath)
    {
      return FileSystem.FileSystem.Local.Directory.MapPath(virtualPath, WebRootPath);
    }

    #endregion

    #endregion
  }
}

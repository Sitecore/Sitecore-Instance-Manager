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
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  [Serializable]
  public class Instance : Website, IXmlSerializable
  {
    #region Static constants, fields, properties and methods
    
    #region Public methods

    public static void DoSmth()
    {
    }

    #endregion

    #endregion

    #region Instance fields

    protected RuntimeSettingsAccessor runtimeSettingsAccessor;
    private InstanceConfiguration configuration;

    private Product product;

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
        return this.runtimeSettingsAccessor ??
               (this.runtimeSettingsAccessor = RuntimeSettingsManager.GetRealtimeSettingsAccessor(this));
      }
    }

    #endregion

    #region Public Properties

    #region Public properties

    public virtual ICollection<Database> AttachedDatabases
    {
      get
      {
        return this.GetAttachedDatabases();
      }
    }

    public virtual IEnumerable<InstanceBackup> Backups
    {
      get
      {
        return this.GetBackups();
      }
    }

    public virtual string BackupsFolder
    {
      get
      {
        return this.GetBackupsFolder();
      }
    }

    [NotNull]
    public virtual InstanceConfiguration Configuration
    {
      get
      {
        return this.configuration ?? (this.configuration = new InstanceConfiguration(this));
      }
    }

    public virtual string CurrentLogFilePath
    {
      get
      {
        return this.GetCurrentLogFilePath();
      }
    }

    public virtual string DataFolderPath
    {
      get
      {
        return this.GetDataFolderPath();
      }
    }

    [CanBeNull]
    public virtual string IndexesFolderPath
    {
      get
      {
        try
        {
          var indexFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("IndexFolder");
          Assert.IsNotNull(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> value is empty string");
          return this.MapPath(indexFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Indexes");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, "Cannot get indexes folder of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get indexes folder of " + this.WebRootPath);
        }
      }
    }

    public virtual bool IsSitecore
    {
      get
      {
        return this.GetIsSitecore();
      }
    }

    [CanBeNull]
    public virtual string LicencePath
    {
      get
      {
        try
        {
          var licenseFile = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("LicenseFile");
          Assert.IsNotNull(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> value is empty string");
          return this.MapPath(licenseFile);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "license.xml");
          if (FileSystem.FileSystem.Local.File.Exists(rootData))
          {
            Log.Error(ex, "Cannot get license file of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get license file of " + this.WebRootPath);
        }
      }
    }

    [NotNull]
    public virtual string LogsFolderPath
    {
      get
      {
        return this.GetLogsFolderPath();
      }
    }

    public ICollection<MongoDbDatabase> MongoDatabases
    {
      get
      {
        return this.GetMongoDatabases();
      }
    }

    [CanBeNull]
    public virtual string PackagesFolderPath
    {
      get
      {
        try
        {
          var packagePath = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("PackagePath");
          Assert.IsNotNull(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> value is empty string");
          return this.MapPath(packagePath);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Packages");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, "Cannot get packages folder of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get packages folder of " + this.WebRootPath);
        }
      }
    }

    [NotNull]
    public virtual Product Product
    {
      get
      {
        return this.product ?? (this.product = ProductManager.GetProduct(this.ProductFullName));
      }
    }

    [NotNull]
    public virtual string ProductFullName
    {
      get
      {
        return ProductHelper.DetectProductFullName(this.WebRootPath);
      }
    }

    public virtual string RootPath
    {
      get
      {
        return this.GetRootPath();
      }
    }

    [CanBeNull]
    public virtual string SerializationFolderPath
    {
      get
      {
        try
        {
          var serializationFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("SerializationFolder");
          Assert.IsNotNull(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> value is empty string");
          return this.MapPath(serializationFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Serialization");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, "Cannot get serialization folder of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get serialization folder of " + this.WebRootPath);
        }
      }
    }

    public InstanceState State
    {
      get
      {
        if (this.IsDisabled)
        {
          return InstanceState.Disabled;
        }

        if (this.ApplicationPoolState == ObjectState.Stopped || this.ApplicationPoolState == ObjectState.Stopping)
        {
          return InstanceState.Stopped;
        }

        if (this.ProcessIds.Any())
        {
          return InstanceState.Running;
        }

        return InstanceState.Ready;
      }
    }

    public virtual bool SupportsCaching
    {
      get
      {
        return false;
      }
    }

    public virtual string TempFolderPath
    {
      get
      {
        return this.GetTempFolderPath();
      }
    }

    public virtual IEnumerable<string> VisualStudioSolutionFiles
    {
      get
      {
        return this.GetVisualStudioSolutionFiles();
      }
    }

    #endregion

    #region Private methods

    private ICollection<MongoDbDatabase> GetMongoDatabases()
    {
      using (new ProfileSection("Get mongo databases", this))
      {
        return this.RuntimeSettingsAccessor.GetMongoDatabases();
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
      foreach (var property in this.GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml("<Instance>" + xml.OuterXml + "</Instance>")), false);
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
        return string.Format("{0} ({1})", base.ToString(), this.ProductFullName);
      }
    }

    [UsedImplicitly]
    public virtual string ModulesNames
    {
      get
      {
        // TODO: replace with modules detector implemenation
        var modulesNames = Directory.GetFiles(this.PackagesFolderPath, "*.zip").Select(x => ProductManager.GetProduct(x)).Where(x => x != Product.Undefined).Select(x => x.Name.TrimStart("Sitecore "));
        return (string.Join(", ", modulesNames) + (File.Exists(Path.Combine(this.WebRootPath, "App_Config\\Include\\Sitecore.Analytics.config")) ? ", DMS" : string.Empty)).TrimStart(" ,".ToCharArray());
      }
    }

    #endregion

    #region Public methods

    [NotNull]
    public virtual string GetBackupFolder(string name)
    {
      string backups = this.GetBackupsFolder();
      return Path.Combine(backups, name);
    }

    public virtual Instance GetCachedInstance()
    {
      return new PartiallyCachedInstance(this);
    }

    [NotNull]
    public virtual XmlDocument GetShowconfig(bool normalize = false)
    {
      using (new ProfileSection("Get showconfig.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return this.RuntimeSettingsAccessor.GetShowconfig(normalize);
      }
    }

    [CanBeNull]
    public virtual IEnumerable<string> GetVisualStudioSolutionFiles(string searchPattern = null)
    {
      var rootPath = this.RootPath;
      var webRootPath = this.WebRootPath;
      return VisualStudioHelper.GetVisualStudioSolutionFiles(rootPath, webRootPath, searchPattern).ToArray();
    }

    [NotNull]
    public virtual XmlDocument GetWebResultConfig(bool normalize = false)
    {
      using (new ProfileSection("Get web.config.result.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return this.RuntimeSettingsAccessor.GetWebConfigResult(normalize);
      }
    }

    public override string ToString()
    {
      return this.DisplayName;
    }

    #endregion

    #endregion

    #region Non-public Methods

    #region Public methods

    public virtual string GetCurrentLogFilePath()
    {
      using (new ProfileSection("Get current log file path", this))
      {
        var logs = this.LogsFolderPath;
        var files = FileSystem.FileSystem.Local.Directory.GetFiles(logs, "log*.txt").OrderBy(FileSystem.FileSystem.Local.File.GetCreationTimeUtc);
        string lastOrDefault = files.LastOrDefault();

        return ProfileSection.Result(lastOrDefault);
      }
    }

    #endregion

    #region Protected methods

    [NotNull]
    protected virtual ICollection<Database> GetAttachedDatabases()
    {
      using (new ProfileSection("Get attached databases", this))
      {
        return this.RuntimeSettingsAccessor.GetDatabases();
      }
    }

    [CanBeNull]
    protected virtual IEnumerable<InstanceBackup> GetBackups()
    {
      var root = this.SafeCall(this.GetBackupsFolder);
      if (string.IsNullOrEmpty(root) || !FileSystem.FileSystem.Local.Directory.Exists(root))
      {
        yield break;
      }

      foreach (var child in FileSystem.FileSystem.Local.Directory.GetDirectories(root))
      {
        var childInfo = new DirectoryInfo(child);
        var date = string.Format("\"{2}\", {0}, {1:hh:mm:ss tt}", childInfo.CreationTime.ToString("yyyy-MM-dd"), 
          childInfo.CreationTime, childInfo.Name);

        if (string.IsNullOrEmpty(date))
        {
          continue;
        }

        var backup = new InstanceBackup(date, child);
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
      string rootPath = this.GetRootPath();
      if (this.WebRootPath.EqualsIgnoreCase(rootPath))
      {
        DirectoryInfo parent = new DirectoryInfo(rootPath).Parent;
        Assert.IsNotNull(parent, "Instance isn't permitted to use drive root as web root path");
        return Path.Combine(parent.FullName, this.Name + " backups");
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
          string dataFolder = this.RuntimeSettingsAccessor.GetScVariableValue("dataFolder");
          Assert.IsNotNull(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element value is empty string");

          return this.MapPath(dataFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(Path.GetDirectoryName(this.WebRootPath), "Data");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, "Cannot get data folder of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get data folder of " + this.WebRootPath);
        }
      }
    }

    protected virtual bool GetIsSitecore()
    {
      try
      {
        return FileSystem.FileSystem.Local.File.Exists(ProductHelper.GetKernelPath(this.WebRootPath));
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during checking if it is sitecore");

        return false;
      }
    }

    protected virtual string GetLogsFolderPath()
    {
      using (new ProfileSection("Get log folder path", this))
      {
        try
        {
          string dataFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("LogFolder");
          var result = this.MapPath(dataFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var dataLogs = Path.Combine(this.DataFolderPath, "logs");
          if (FileSystem.FileSystem.Local.Directory.Exists(dataLogs))
          {
            Log.Error(ex, "Cannot get logs folder of {0}",  this.WebRootPath);

            return dataLogs;
          }

          throw new InvalidOperationException("Cannot get logs folder of " + this.WebRootPath);
        }
      }
    }

    protected virtual string GetRootFolderViaDatabases(ICollection<Database> databases)
    {
      string webRootPath = this.WebRootPath;
      using (new ProfileSection("Get root folder (using databases)", this))
      {
        ProfileSection.Argument("databases", databases);

        foreach (var database in databases)
        {
          Log.Debug("Database: {0}",  database);
          string fileName = database.FileName;
          if (string.IsNullOrEmpty(fileName))
          {
            Log.Warn("The {0} database seems to be detached since it doesn't have a FileName property filled in", database.RealName);
            continue;
          }

          Log.Debug(
            "name: {0}, fileName: {1}", database.Name, fileName);
          var folder = Path.GetDirectoryName(fileName);
          if (folder.ContainsIgnoreCase(webRootPath))
          {
            continue;
          }

          Assert.IsNotNullOrEmpty(folder, "folder1");
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
          var webRootPath = this.WebRootPath;
          string dataFolderPath = this.GetDataFolderPath();
          Assert.IsNotNullOrEmpty(dataFolderPath, "dataFolderPath");

          // data folder is inside website folder
          if (dataFolderPath.ContainsIgnoreCase(this.WebRootPath))
          {
            // find using only databases
            var result = this.GetRootFolderViaDatabases(this.GetAttachedDatabases()) ?? webRootPath;

            return ProfileSection.Result(result);
          }


          // trying to detect using databases
          var common = this.GetRootFolderViaDatabases(this.GetAttachedDatabases());
          if (common != null)
          {
            return ProfileSection.Result(common);
          }

          // trying to detect via data folder
          string detectedRoot = FileSystem.FileSystem.Local.Directory.FindCommonParent(webRootPath, dataFolderPath);

          // if impossible
          if (string.IsNullOrEmpty(detectedRoot))
          {
            return ProfileSection.Result(webRootPath);
          }

          InvalidConfigurationException.Assert(!webRootPath.ContainsIgnoreCase(dataFolderPath), 
            "The data folder accidentally was set to be parent ({0}) of the website root folder ({1})".FormatWith(
              dataFolderPath, webRootPath));
          int distance = FileSystem.FileSystem.Local.Directory.GetDistance(webRootPath, detectedRoot);
          InvalidConfigurationException.Assert(distance <= 1, 
            "Cannot detect the Root Folder - the detection result ({1}) is too far from the Website ({0}) folder"
              .FormatWith(this.WebRootPath, detectedRoot));

          return ProfileSection.Result(detectedRoot);
        }
        catch (Exception ex)
        {
          var rootData = Path.GetDirectoryName(this.WebRootPath);
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error(ex, "Cannot get root folder of {0}",  this.WebRootPath);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get root folder of " + this.WebRootPath);
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
          string tempFolder = this.RuntimeSettingsAccessor.GetScVariableValue("tempFolder");
          Assert.IsNotNull(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element value is empty string");

          var result = this.MapPath(tempFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var websiteTemp = Path.Combine(this.WebRootPath, "temp");
          if (FileSystem.FileSystem.Local.Directory.Exists(websiteTemp))
          {
            Log.Error(ex, "Cannot get temp folder of {0}",  this.WebRootPath);

            return websiteTemp;
          }

          throw new InvalidOperationException("Cannot get temp folder of " + this.WebRootPath);
        }
      }
    }

    protected virtual string MapPath(string virtualPath)
    {
      return FileSystem.FileSystem.Local.Directory.MapPath(virtualPath, this.WebRootPath);
    }

    #endregion

    #endregion
  }
}
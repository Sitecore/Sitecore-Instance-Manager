#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SIM.Adapters;
using SIM.Adapters.SqlServer;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances.RuntimeSettings;
using SIM.Products;
using Sitecore.ConfigBuilder;
using Assert = SIM.Base.Assert;

#endregion

namespace SIM.Instances
{
  using Microsoft.Web.Administration;
  using SIM.Adapters.MongoDb;

  /// <summary>
  ///   The instance.
  /// </summary>
  [Serializable]
  public class Instance : Website, IXmlSerializable
  {
    #region Static constants, fields, properties and methods

    private static int fld22;
    public static int fld;

    public static void DoSmth()
    {
    }

    #endregion

    #region Instance fields

    private InstanceConfiguration configuration;

    private Product product;
    protected RuntimeSettingsAccessor runtimeSettingsAccessor;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Instance"/> class.
    /// </summary>
    /// <param name="id">
    /// The id. 
    /// </param>
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

    public virtual string BackupsFolder
    {
      get { return GetBackupsFolder(); }
    }

    public virtual string DataFolderPath
    {
      get { return GetDataFolderPath(); }
    }

    public virtual IEnumerable<Database> AttachedDatabases
    {
      get { return GetAttachedDatabases(); }
    }

    public virtual bool IsSitecore
    {
      get { return GetIsSitecore(); }
    }

    [NotNull]
    public virtual InstanceConfiguration Configuration
    {
      get { return this.configuration ?? (this.configuration = new InstanceConfiguration(this)); }
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
          return this.MapPath(licenseFile);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "license.xml");
          if (FileSystem.Local.File.Exists(rootData))
          {
            Log.Error("Cannot get license file of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get license file of " + this.WebRootPath, ex);
        }
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
          return this.MapPath(packagePath);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Packages");
          if (FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get packages folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get packages folder of " + this.WebRootPath, ex);
        }
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
          return this.MapPath(indexFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Indexes");
          if (FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get indexes folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get indexes folder of " + this.WebRootPath, ex);
        }
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
          return this.MapPath(serializationFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Serialization");
          if (FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get serialization folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get serialization folder of " + this.WebRootPath, ex);
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

    /// <summary>
    ///   Should be called only if IsSitecore is true
    /// </summary>
    [NotNull]
    public virtual Product Product
    {
      get { return this.product ?? (this.product = ProductManager.GetProduct(this.ProductFullName)); }
    }

    /// <summary>
    ///   Should be called only if IsSitecore is true
    /// </summary>
    [NotNull]
    public virtual string ProductFullName
    {
      get { return ProductHelper.DetectProductFullName(this.WebRootPath); }
    }

    public virtual string RootPath
    {
      get { return GetRootPath(); }
    }

    public virtual string TempFolderPath
    {
      get { return GetTempFolderPath(); }
    }

    [NotNull]
    public virtual string LogsFolderPath
    {
      get { return GetLogsFolderPath(); }
    }

    public virtual string CurrentLogFilePath
    {
      get { return GetCurrentLogFilePath(); }
    }

    public virtual IEnumerable<string> VisualStudioSolutionFiles
    {
      get { return GetVisualStudioSolutionFiles(); }
    }

    public virtual IEnumerable<InstanceBackup> Backups
    {
      get { return GetBackups(); }
    }


    public virtual bool SupportsCaching
    {
      get { return false; }
    }

    public ICollection<MongoDbDatabase> MongoDatabases
    {
      get
      {
        return this.GetMongoDatabases();
      }
    }

    private ICollection<MongoDbDatabase> GetMongoDatabases()
    {
      using (new ProfileSection("Get mongo databases", this))
      {
        return RuntimeSettingsAccessor.GetMongoDatabases();
      }
    }

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

    /// <summary>
    /// The get backup folder.
    /// </summary>
    /// <param name="dateTime">
    /// The date time. 
    /// </param>
    /// <returns>
    /// The get backup folder. 
    /// </returns>
    [NotNull]
    public virtual string GetBackupFolder(string name)
    {
      string backups = this.GetBackupsFolder();
      return Path.Combine(backups, name);
    }

    /// <summary>
    /// Usually Instance object is realtime, which means that it accesses IIS, filesystem etc to get particular info for each time. 
    /// Sometimes you might allow caching during some session for performance reasons (e.g. during some excessive operation execution). 
    /// This method returns identical instance but with caching support.
    /// </summary>
    /// <returns></returns>
    public virtual Instance GetCachedInstance()
    {
      return new PartiallyCachedInstance(this);
    }

    [CanBeNull]
    public virtual IEnumerable<string> GetVisualStudioSolutionFiles(string searchPattern = null)
    {
      var rootPath = this.RootPath;
      var webRootPath = this.WebRootPath;
      return VisualStudioHelper.GetVisualStudioSolutionFiles(rootPath, webRootPath, searchPattern).ToArray();
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

    [NotNull]
    public virtual XmlDocument GetWebResultConfig(bool normalize = false)
    {
      using (new ProfileSection("Get web.config.result.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return RuntimeSettingsAccessor.GetWebConfigResult(normalize);
      }
    }

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The to string </returns>
    public override string ToString()
    {
      return this.DisplayName;
    }

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
        //TODO: replace with modules detector implemenation
        var modulesNames = Directory.GetFiles(this.PackagesFolderPath, "*.zip").Select(x => ProductManager.GetProduct(x)).Where(x => x != Product.Undefined).Select(x => x.Name.TrimStart("Sitecore "));
        return (string.Join(", ", modulesNames) + (File.Exists(Path.Combine(this.WebRootPath, "App_Config\\Include\\Sitecore.Analytics.config")) ? ", DMS" : "")).TrimStart(" ,".ToCharArray());
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
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).Name".FormatWith(ID)))
        {
          context.Sites[name].Name = name + "_disabled";
          context.CommitChanges();
        }
      }
    }

    #endregion

    #region Non-public Methods

    [NotNull]
    protected virtual ICollection<Database> GetAttachedDatabases()
    {
      using (new ProfileSection("Get attached databases", this))
      {
        return RuntimeSettingsAccessor.GetDatabases();
      }
    }

    /// <summary>
    ///   The get backups.
    /// </summary>
    /// <returns> Returns the backups of the instance </returns>
    [CanBeNull]
    protected virtual IEnumerable<InstanceBackup> GetBackups()
    {
      var root = this.SafeCall(this.GetBackupsFolder);
      if (string.IsNullOrEmpty(root) || !FileSystem.Local.Directory.Exists(root)) yield break;

      foreach (var child in FileSystem.Local.Directory.GetDirectories(root))
      {
        var childInfo = new DirectoryInfo(child);
        var date = string.Format("\"{2}\", {0}, {1:hh:mm:ss tt}", childInfo.CreationTime.ToString("yyyy-MM-dd"),
          childInfo.CreationTime, childInfo.Name);

        if (string.IsNullOrEmpty(date)) continue;

        var backup = new InstanceBackup(date, child);
        if (!FileSystem.Local.Directory.Exists(backup.DatabasesFolderPath) && !FileSystem.Local.Directory.Exists(backup.MongoDatabasesFolderPath) && !FileSystem.Local.File.Exists(backup.WebRootFilePath) &&
            !FileSystem.Local.File.Exists(backup.WebRootNoClientFilePath)) continue;

        yield return backup;
      }
    }

    [NotNull]
    protected virtual string GetBackupsFolder()
    {
      string rootPath = this.GetRootPath();
      if (WebRootPath.EqualsIgnoreCase(rootPath))
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
          string dataFolder = RuntimeSettingsAccessor.GetScVariableValue("dataFolder");
          Assert.IsNotNull(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element value is empty string");

          return this.MapPath(dataFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(Path.GetDirectoryName(this.WebRootPath), "Data");
          if (FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get data folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get data folder of " + this.WebRootPath, ex);
        }
      }
    }

    protected virtual bool GetIsSitecore()
    {
      try
      {
        return FileSystem.Local.File.Exists(ProductHelper.GetKernelPath(this.WebRootPath));
      }
      catch (Exception ex)
      {
        Log.Warn("An error occurred during checking if it is sitecore", this, ex);

        return false;
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
          Log.Debug("Database: " + database);
          string fileName = database.FileName;
          if (string.IsNullOrEmpty(fileName))
          {
            Log.Warn(
              "The {0} database seems to be detached since it doesn't have a FileName property filled in".FormatWith(
                database.RealName), typeof(string));
            continue;
          }
          Log.Debug(
            "name: {0}, fileName: {1}".FormatWith(database.Name, fileName),
            typeof(Instance));
          var folder = Path.GetDirectoryName(fileName);
          if (folder.ContainsIgnoreCase(webRootPath)) continue;
          Assert.IsNotNullOrEmpty(folder, "folder1");
          var common = FileSystem.Local.Directory.FindCommonParent(webRootPath, folder);
          if (string.IsNullOrEmpty(common)) continue;
          if (Math.Abs(FileSystem.Local.Directory.GetDistance(webRootPath, common)) <= 1)
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
          string detectedRoot = FileSystem.Local.Directory.FindCommonParent(webRootPath, dataFolderPath);

          // if impossible
          if (string.IsNullOrEmpty(detectedRoot))
          {
            return ProfileSection.Result(webRootPath);
          }

          InvalidConfigurationException.Assert(!webRootPath.ContainsIgnoreCase(dataFolderPath),
            "The data folder accidentally was set to be parent ({0}) of the website root folder ({1})".FormatWith(
              dataFolderPath, webRootPath));
          int distance = FileSystem.Local.Directory.GetDistance(webRootPath, detectedRoot);
          InvalidConfigurationException.Assert(distance <= 1,
            "Cannot detect the Root Folder - the detection result ({1}) is too far from the Website ({0}) folder"
              .FormatWith(WebRootPath, detectedRoot));

          return ProfileSection.Result(detectedRoot);
        }
        catch (Exception ex)
        {
          var rootData = Path.GetDirectoryName(this.WebRootPath);
          if (FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get root folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get root folder of " + this.WebRootPath, ex);
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
          string tempFolder = RuntimeSettingsAccessor.GetScVariableValue("tempFolder");
          Assert.IsNotNull(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element value is empty string");

          var result = this.MapPath(tempFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var websiteTemp = Path.Combine(this.WebRootPath, "temp");
          if (FileSystem.Local.Directory.Exists(websiteTemp))
          {
            Log.Error("Cannot get temp folder of " + this.WebRootPath, this, ex);

            return websiteTemp;
          }

          throw new InvalidOperationException("Cannot get temp folder of " + this.WebRootPath, ex);
        }
      }
    }

    protected virtual string GetLogsFolderPath()
    {
      using (new ProfileSection("Get log folder path", this))
      {
        try
        {
          string dataFolder = RuntimeSettingsAccessor.GetSitecoreSettingValue("LogFolder");
          var result = this.MapPath(dataFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var dataLogs = Path.Combine(this.DataFolderPath, "logs");
          if (FileSystem.Local.Directory.Exists(dataLogs))
          {
            Log.Error("Cannot get logs folder of " + this.WebRootPath, this, ex);

            return dataLogs;
          }

          throw new InvalidOperationException("Cannot get logs folder of " + this.WebRootPath, ex);
        }
      }
    }

    public virtual string GetCurrentLogFilePath()
    {
      using (new ProfileSection("Get current log file path", this))
      {
        var logs = this.LogsFolderPath;
        var files = FileSystem.Local.Directory.GetFiles(logs, "log*.txt").OrderBy(FileSystem.Local.File.GetCreationTimeUtc);
        string lastOrDefault = files.LastOrDefault();

        return ProfileSection.Result(lastOrDefault);
      }
    }

    protected virtual string MapPath(string virtualPath)
    {
      return FileSystem.Local.Directory.MapPath(virtualPath, this.WebRootPath);
    }

    #endregion
  }
}
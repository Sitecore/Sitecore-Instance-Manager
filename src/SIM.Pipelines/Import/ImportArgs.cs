namespace SIM.Pipelines.Import
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ImportArgs : ProcessorArgs
  {
    #region Constants

    public const string appPoolSettingsFileName = "AppPoolSettings.xml";
    public const string websiteSettingsFileName = "WebsiteSettings.xml";

    #endregion

    // Site settings
    // public List<string> siteBindingsHostnames = new List<string>();
    #region Fields

    public string appPoolName = string.Empty;
    public Dictionary<string, int> bindings = new Dictionary<string, int>();
    public SqlConnectionStringBuilder connectionString;
    public int databaseNameAppend = -1;
    public string oldSiteName = string.Empty;
    public string pathToLicenseFile = string.Empty;
    public string rootPath = string.Empty;
    public long? siteID = 0;
    public string siteName = string.Empty;
    public string temporaryPathToUnpack = string.Empty;
    public bool updateLicense = false;
    public string virtualDirectoryPath = string.Empty;
    public string virtualDirectoryPhysicalPath = string.Empty;

    #endregion

    #region Properties

    public string PathToExportedInstance { get; set; }

    #endregion

    #region Constructors

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(pathToExportedInstance, "pathToExportedInstance");
      this.PathToExportedInstance = pathToExportedInstance;
      this.connectionString = connectionString;
    }

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] string siteName, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(pathToExportedInstance, "pathToExportedInstance");
      this.PathToExportedInstance = pathToExportedInstance;
      this.siteName = siteName;
      this.connectionString = connectionString;
    }

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] string siteName, [NotNull] string temporaryPathToUnpack, [NotNull] string rootPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] bool updateLicense, [NotNull] string pathToLicenseFile, [NotNull] Dictionary<string, int> bindings)
    {
      Assert.ArgumentNotNull(pathToExportedInstance, "pathToExportedInstance");
      this.PathToExportedInstance = pathToExportedInstance;
      this.siteName = siteName;
      this.temporaryPathToUnpack = temporaryPathToUnpack;
      this.rootPath = rootPath;
      this.virtualDirectoryPhysicalPath = this.rootPath.PathCombine("Website");
      this.connectionString = connectionString;
      this.updateLicense = updateLicense;
      this.pathToLicenseFile = pathToLicenseFile;
      this.bindings = bindings;
    }

    #endregion
  }
}
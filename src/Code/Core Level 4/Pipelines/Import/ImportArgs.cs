#region Usings

using System;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;
using System.Collections.Generic;

#endregion

namespace SIM.Pipelines.Import
{
	/// <summary>
	///   The import args.
	/// </summary>
	public class ImportArgs : ProcessorArgs
	{
		#region Fields

        public const string websiteSettingsFileName = "WebsiteSettings.xml";
        public const string appPoolSettingsFileName = "AppPoolSettings.xml";
        //Site settings
        public string siteName = "";
        public string oldSiteName = "";
        //public List<string> siteBindingsHostnames = new List<string>();
        public string appPoolName = "";
        public string virtualDirectoryPath = "";
        public string virtualDirectoryPhysicalPath = "";
        public string rootPath = "";
        public string temporaryPathToUnpack = "";
        public int databaseNameAppend = -1;
        public long? siteID = 0;
        public System.Data.SqlClient.SqlConnectionStringBuilder connectionString;
        public bool updateLicense = false;
        public string pathToLicenseFile = "";
        public Dictionary<string, int> bindings = new Dictionary<string, int>();
        
		#endregion
		 
		#region Properties

        public string PathToExportedInstance { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SIM.Pipelines.Export.ExportArgs"/> class.
		/// </summary>
		/// <param name="instance">
		/// The instance. 
		/// </param>

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] System.Data.SqlClient.SqlConnectionStringBuilder connectionString)
		{
      Assert.ArgumentNotNull(pathToExportedInstance, "pathToExportedInstance");
      this.PathToExportedInstance = pathToExportedInstance;
      this.connectionString = connectionString;  
		}

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] string siteName , [NotNull] System.Data.SqlClient.SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(pathToExportedInstance, "pathToExportedInstance");
      this.PathToExportedInstance = pathToExportedInstance;
      this.siteName = siteName;
      this.connectionString = connectionString;

    }

    public ImportArgs([NotNull] string pathToExportedInstance, [NotNull] string siteName, [NotNull] string temporaryPathToUnpack, [NotNull] string rootPath, [NotNull] System.Data.SqlClient.SqlConnectionStringBuilder connectionString, [NotNull] bool updateLicense, [NotNull] string pathToLicenseFile, [NotNull] Dictionary<string, int> bindings)
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

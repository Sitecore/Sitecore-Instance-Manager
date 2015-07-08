#region Usings

using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using SIM.Adapters;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;
using SIM.Products;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The install args.
  /// </summary>
  public class InstallArgs : ProcessorArgs
  {
    #region Fields

    /// <summary>
    ///   The connection string.
    /// </summary>
    [NotNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    /// <summary>
    ///   The data folder path.
    /// </summary>
    [NotNull]
    public readonly string DataFolderPath;

    /// <summary>
    ///   The root folder path.
    /// </summary>
    [NotNull]
    public readonly string RootFolderPath;

    /// <summary>
    ///   The databases folder path.
    /// </summary>
    [NotNull]
    public readonly string DatabasesFolderPath;

    /// <summary>
    ///   The force net framework 4.
    /// </summary>
    public readonly bool ForceNetFramework4;

    /// <summary>
    ///   The host name.
    /// </summary>
    [NotNull]
    public readonly string HostName;

    /// <summary>
    ///   The is 32 bit.
    /// </summary>
    public readonly bool Is32Bit;

    /// <summary>
    ///   The is classic.
    /// </summary>
    public readonly bool IsClassic;

    /// <summary>
    ///   The license file path.
    /// </summary>
    [NotNull]
    public readonly string LicenseFilePath;

    /// <summary>
    ///   The modules.
    /// </summary>
    [NotNull]
    public readonly IEnumerable<Product> Modules;

    /// <summary>
    ///   The name.
    /// </summary>
    [NotNull]
    public readonly string Name;

    /// <summary>
    ///   The product.
    /// </summary>
    [NotNull]
    public readonly Product Product;

    /// <summary>
    ///   The sql server identity.
    /// </summary>
    [NotNull]
    public readonly string SqlServerIdentity;

    /// <summary>
    ///   The unique temp folder.
    /// </summary>
    [NotNull]
    public readonly string UniqueTempFolder;

    /// <summary>
    ///   The web root path.
    /// </summary>
    [NotNull]
    public readonly string WebRootPath;

    /// <summary>
    ///   The web server identity.
    /// </summary>
    [NotNull]
    public readonly string WebServerIdentity;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InstallArgs"/> class.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="host">
    /// The host. 
    /// </param>
    /// <param name="product">
    /// The product. 
    /// </param>
    /// <param name="rootPath">
    /// The root path. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="sqlServerIdentity">
    /// The sql server identity. 
    /// </param>
    /// <param name="webServerIdentity">
    /// The web server identity. 
    /// </param>
    /// <param name="license">
    /// The license. 
    /// </param>
    /// <param name="forceNetFramework4">
    /// The force net framework 4. 
    /// </param>
    /// <param name="is32Bit">
    /// The is 32 bit. 
    /// </param>
    /// <param name="isClassic">
    /// The is classic. 
    /// </param>
    /// <param name="modules">
    /// The modules. 
    /// </param>
    public InstallArgs([NotNull] string name, [NotNull] string host, [NotNull] Product product, [NotNull] string rootPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, [NotNull] IEnumerable<Product> modules)
      : this(name, host, product, Path.Combine(rootPath, "Website"), Path.Combine(rootPath, "Data"), Path.Combine(rootPath, "Databases"), connectionString, sqlServerIdentity, webServerIdentity, license, forceNetFramework4, is32Bit, isClassic, rootPath, modules)
    {
      Assert.ArgumentNotNull(name, "name");
      Assert.ArgumentNotNull(host, "host");
      Assert.ArgumentNotNull(product, "product");
      Assert.ArgumentNotNull(rootPath, "rootPath");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(sqlServerIdentity, "sqlServerIdentity");
      Assert.ArgumentNotNull(webServerIdentity, "webServerIdentity");
      Assert.ArgumentNotNull(license, "license");
      Assert.ArgumentNotNull(modules, "modules");

      this.Modules = modules;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstallArgs"/> class.
    /// </summary>
    /// <param name="name">
    ///   The name. 
    /// </param>
    /// <param name="host">
    ///   The host. 
    /// </param>
    /// <param name="product">
    ///   The product. 
    /// </param>
    /// <param name="webRootPath">
    ///   The web root path. 
    /// </param>
    /// <param name="dataFolderPath">
    ///   The data folder path. 
    /// </param>
    /// <param name="databasesFolderPath">
    ///   The databases folder path. 
    /// </param>
    /// <param name="connectionString">
    ///   The connection string. 
    /// </param>
    /// <param name="sqlServerIdentity">
    ///   The sql server identity. 
    /// </param>
    /// <param name="webServerIdentity">
    ///   The web server identity. 
    /// </param>
    /// <param name="license">
    ///   The license. 
    /// </param>
    /// <param name="forceNetFramework4">
    ///   The force net framework 4. 
    /// </param>
    /// <param name="is32Bit">
    ///   The is 32 bit. 
    /// </param>
    /// <param name="isClassic">
    ///   The is classic. 
    /// </param>
    /// <param name="rootPath"></param>
    /// <param name="modules">
    ///   The modules. 
    /// </param>
    public InstallArgs([NotNull] string name, [NotNull] string host, [NotNull] Product product, [NotNull] string webRootPath, [NotNull] string dataFolderPath, [NotNull] string databasesFolderPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, [NotNull] string rootPath, [NotNull] IEnumerable<Product> modules)
    {
      Assert.ArgumentNotNull(name, "name");
      Assert.ArgumentNotNull(host, "host");
      Assert.ArgumentNotNull(product, "product");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      Assert.ArgumentNotNull(dataFolderPath, "dataFolderPath");
      Assert.ArgumentNotNull(databasesFolderPath, "databasesFolderPath");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      Assert.ArgumentNotNull(sqlServerIdentity, "sqlServerIdentity");
      Assert.ArgumentNotNull(webServerIdentity, "webServerIdentity");
      Assert.ArgumentNotNull(license, "license");
      Assert.ArgumentNotNull(rootPath, "rootPath");
      Assert.ArgumentNotNull(modules, "modules");

      string uniqueTempFolder = FileSystem.Local.Directory.GenerateTempFolderPath(Settings.CoreInstallTempFolderLocation.Value.EmptyToNull() ?? Path.GetPathRoot(webRootPath));
      FileSystem.Local.Directory.Ensure(uniqueTempFolder);

      this.Name = name;
      this.Modules = modules;
      this.HostName = host;
      this.Product = product;
      this.ConnectionString = connectionString;
      this.DataFolderPath = dataFolderPath;
      this.DatabasesFolderPath = databasesFolderPath;
      this.WebRootPath = webRootPath;
      this.UniqueTempFolder = uniqueTempFolder;
      this.LicenseFilePath = license.FullName;
      this.SqlServerIdentity = sqlServerIdentity;
      this.WebServerIdentity = webServerIdentity;
      this.ForceNetFramework4 = forceNetFramework4;
      this.Is32Bit = is32Bit;
      this.IsClassic = isClassic;
      this.RootFolderPath = rootPath;
    }

    #endregion

    #region Properties

    #region Public properties

    /// <summary>
    ///   Gets InstanceName.
    /// </summary>
    [NotNull]
    public string InstanceName
    {
      get
      {
        return this.Name;
      }
    }

    #endregion

    /// <summary>
    ///   Gets or sets Instance.
    /// </summary>
    [CanBeNull]
    public Instance Instance { get; set; }

    /// <summary>
    ///   Gets PackagePath.
    /// </summary>
    [NotNull]
    public string PackagePath
    {
      get
      {
        return this.Product.PackagePath;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///   The dispose.
    /// </summary>
    public override void Dispose()
    {
      FileSystem.Local.Directory.TryDelete(this.UniqueTempFolder);

      base.Dispose();
    }

    #endregion

    // for install pipeline
  }
}
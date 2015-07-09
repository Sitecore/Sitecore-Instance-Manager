namespace SIM.Pipelines.Install
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  public class InstallArgs : ProcessorArgs
  {
    #region Fields

    [NotNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    [NotNull]
    public readonly string DataFolderPath;

    [NotNull]
    public readonly string DatabasesFolderPath;

    public readonly bool ForceNetFramework4;

    [NotNull]
    public readonly string HostName;

    public readonly bool Is32Bit;

    public readonly bool IsClassic;

    [NotNull]
    public readonly string LicenseFilePath;

    [NotNull]
    public readonly IEnumerable<Product> Modules;

    [NotNull]
    public readonly string Name;

    [NotNull]
    public readonly Product Product;

    [NotNull]
    public readonly string RootFolderPath;

    [NotNull]
    public readonly string SqlServerIdentity;

    [NotNull]
    public readonly string UniqueTempFolder;

    [NotNull]
    public readonly string WebRootPath;

    [NotNull]
    public readonly string WebServerIdentity;

    #endregion

    #region Constructors

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

      string uniqueTempFolder = FileSystem.FileSystem.Local.Directory.GenerateTempFolderPath(Settings.CoreInstallTempFolderLocation.Value.EmptyToNull() ?? Path.GetPathRoot(webRootPath));
      FileSystem.FileSystem.Local.Directory.Ensure(uniqueTempFolder);

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

    [CanBeNull]
    public Instance Instance { get; set; }

    [NotNull]
    public string InstanceName
    {
      get
      {
        return this.Name;
      }
    }

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

    public override void Dispose()
    {
      FileSystem.FileSystem.Local.Directory.TryDelete(this.UniqueTempFolder);

      base.Dispose();
    }

    #endregion

    // for install pipeline
  }
}
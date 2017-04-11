namespace SIM.Pipelines.Install
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public class InstallArgs : ProcessorArgs
  {
    #region Fields

    [NotNull]
    public SqlConnectionStringBuilder ConnectionString { get; }

    [NotNull]
    public string DataFolderPath { get; }

    [NotNull]
    public string DatabasesFolderPath { get; }

    public bool ForceNetFramework4 { get; }

    [NotNull]
    public readonly string[] _HostNames;

    public string InstanceSqlPrefix { get; }
    public bool InstanceAttachSql { get; }

    public bool Is32Bit { get; }

    public bool IsClassic { get; }

    public bool PreHeat { get; }

    [NotNull]
    public string LicenseFilePath { get; }

    [NotNull]
    public readonly IEnumerable<Product> _Modules;

    [NotNull]
    public string Name { get; }

    [NotNull]
    public Product Product { get; }

    [NotNull]
    public string RootFolderPath { get; }

    [NotNull]
    public string SqlServerIdentity { get; }

    [NotNull]
    public string WebRootPath { get; }

    [NotNull]
    public string WebServerIdentity { get; }

    public bool InstallRadControls { get; }
    public bool InstallDictionaries { get; }

    #endregion

    #region Constructors

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] string rootPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, [NotNull] IEnumerable<Product> modules)
      : this(name, hosts, instanceSqlPrefix, instanceAttachSql, product, Path.Combine(rootPath, "Website"), Path.Combine(rootPath, "Data"), Path.Combine(rootPath, "Databases"), connectionString, sqlServerIdentity, webServerIdentity, license, forceNetFramework4, is32Bit, isClassic, installRadControls, installDictionaries, serverSideRedirect, increaseExecutionTimeout, preheat, rootPath, modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(modules, nameof(modules));

      _Modules = modules;
    }

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] string webRootPath, [NotNull] string dataFolderPath, [NotNull] string databasesFolderPath, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] FileInfo license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, [NotNull] string rootPath, [NotNull] IEnumerable<Product> modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      Assert.ArgumentNotNull(dataFolderPath, nameof(dataFolderPath));
      Assert.ArgumentNotNull(databasesFolderPath, nameof(databasesFolderPath));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      Assert.ArgumentNotNull(modules, nameof(modules));

      Name = name;
      _Modules = modules;
      _HostNames = hosts;
      InstanceSqlPrefix = instanceSqlPrefix;
      InstanceAttachSql = instanceAttachSql;
      Product = product;
      ConnectionString = connectionString;
      DataFolderPath = dataFolderPath;
      DatabasesFolderPath = databasesFolderPath;
      WebRootPath = webRootPath;
      LicenseFilePath = license.FullName;
      SqlServerIdentity = sqlServerIdentity;
      WebServerIdentity = webServerIdentity;
      ForceNetFramework4 = forceNetFramework4;
      Is32Bit = is32Bit;
      IsClassic = isClassic;
      PreHeat = preheat;
      RootFolderPath = rootPath;
      InstallRadControls = installRadControls;
      InstallDictionaries = installDictionaries;
      ServerSideRedirect = serverSideRedirect;
      IncreaseExecutionTimeout = increaseExecutionTimeout;
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
        return Name;
      }
    }

    [NotNull]
    public string PackagePath
    {
      get
      {
        return Product.PackagePath;
      }
    }

    public bool ServerSideRedirect { get; set; }

    public bool IncreaseExecutionTimeout { get; set; }

    #endregion

    // for install pipeline
  }
}
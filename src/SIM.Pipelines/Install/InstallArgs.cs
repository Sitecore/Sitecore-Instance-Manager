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
  using SIM.IO;

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

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] IFolder rootDir, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] IFile license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, string installRoles8, string installRoles9, [NotNull] IEnumerable<Product> modules)
      : this(name, hosts, instanceSqlPrefix, instanceAttachSql, product, rootDir.GetChildFolder("Website"), rootDir.GetChildFolder("Data"), rootDir.GetChildFolder("Databases"), connectionString, sqlServerIdentity, webServerIdentity, license, forceNetFramework4, is32Bit, isClassic, installRadControls, installDictionaries, serverSideRedirect, increaseExecutionTimeout, preheat, rootDir, installRoles8, installRoles9, modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(rootDir, nameof(rootDir));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(modules, nameof(modules));

      _Modules = modules;
    }

    public InstallArgs([NotNull] string name, [NotNull] string[] hosts, string instanceSqlPrefix, bool instanceAttachSql, [NotNull] Product product, [NotNull] IFolder webRootDir, [NotNull] IFolder dataDir, [NotNull] IFolder databasesDir, [NotNull] SqlConnectionStringBuilder connectionString, [NotNull] string sqlServerIdentity, [NotNull] string webServerIdentity, [NotNull] IFile license, bool forceNetFramework4, bool is32Bit, bool isClassic, bool installRadControls, bool installDictionaries, bool serverSideRedirect, bool increaseExecutionTimeout, bool preheat, [NotNull] IFolder rootDir, string installRoles8, string installRoles9, [NotNull] IEnumerable<Product> modules)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(hosts, nameof(hosts));
      Assert.ArgumentNotNull(product, nameof(product));
      Assert.ArgumentNotNull(webRootDir, nameof(webRootDir));
      Assert.ArgumentNotNull(dataDir, nameof(dataDir));
      Assert.ArgumentNotNull(databasesDir, nameof(databasesDir));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(sqlServerIdentity, nameof(sqlServerIdentity));
      Assert.ArgumentNotNull(webServerIdentity, nameof(webServerIdentity));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(rootDir, nameof(rootDir));
      Assert.ArgumentNotNull(modules, nameof(modules));

      Name = name;
      _Modules = modules;
      _HostNames = hosts;
      InstanceSqlPrefix = instanceSqlPrefix;
      InstanceAttachSql = instanceAttachSql;
      Product = product;
      ConnectionString = connectionString;
      DataFolderPath = dataDir.FullName;
      DatabasesFolderPath = databasesDir.FullName;
      WebRootPath = webRootDir.FullName;
      LicenseFilePath = license.FullName;
      SqlServerIdentity = sqlServerIdentity;
      WebServerIdentity = webServerIdentity;
      ForceNetFramework4 = forceNetFramework4;
      Is32Bit = is32Bit;
      IsClassic = isClassic;
      PreHeat = preheat;
      RootFolderPath = rootDir.FullName;
      InstallRadControls = installRadControls;
      InstallDictionaries = installDictionaries;
      ServerSideRedirect = serverSideRedirect;
      IncreaseExecutionTimeout = increaseExecutionTimeout;
      InstallRoles8 = installRoles8;
      InstallRoles9 = installRoles9;
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
    public string InstallRoles8 { get; set; }
    public string InstallRoles9 { get; set; }

    #endregion

    // for install pipeline
  }
}
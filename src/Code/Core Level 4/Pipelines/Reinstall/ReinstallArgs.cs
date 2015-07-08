#region Usings

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;
using SIM.Products;

#endregion

namespace SIM.Pipelines.Reinstall
{
  


  #region

  using System.Linq;
  using SIM.Adapters.WebServer;

  #endregion

  /// <summary>
  ///   The reinstall args.
  /// </summary>
  public class ReinstallArgs : ProcessorArgs
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
    public readonly IEnumerable<BindingInfo> Bindings;

    /// <summary>
    ///   The instance databases.
    /// </summary>
    [CanBeNull]
    public readonly IEnumerable<Database> InstanceDatabases;

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
    ///   The root path.
    /// </summary>
    public readonly string RootPath;

    /// <summary>
    ///   The instance stopper.
    /// </summary>
    [NotNull]
    public readonly Action<bool> StopInstance;

    /// <summary>
    ///   The temp folder.
    /// </summary>
    public readonly string TempFolder;

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

    /// <summary>
    ///   The website ID.
    /// </summary>
    [NotNull]
    public readonly long WebsiteID;

    /// <summary>
    ///   The website ID.
    /// </summary>
    [NotNull]
    public readonly string instanceName;

    /// <summary>
    ///   Gets InstanceName.
    /// </summary>
    [NotNull]
    public string InstanceName
    {
      get
      {
        return this.instanceName;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReinstallArgs"/> class.
    /// </summary>
    /// <param name="instance">
    /// The instance. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <param name="license">
    /// The license. 
    /// </param>
    /// <param name="webServerIdentity">
    /// The web server identity. 
    /// </param>
    public ReinstallArgs(Instance instance, SqlConnectionStringBuilder connectionString, string license, string webServerIdentity) 
    {
      this.ConnectionString = connectionString;
      this.Name = instance.Name;
      this.Bindings = instance.Bindings;
      this.Product = instance.Product;
      this.WebRootPath = instance.WebRootPath;
      this.RootPath = instance.RootPath;
      this.DataFolderPath = instance.DataFolderPath;
      this.DatabasesFolderPath = Path.Combine(RootPath, "Databases");
      this.WebServerIdentity = webServerIdentity;
      this.LicenseFilePath = license;
      this.Modules = new Product[0];
      this.IsClassic = instance.IsClassic;
      this.Is32Bit = instance.Is32Bit;
      this.ForceNetFramework4 = instance.IsNetFramework4;
      this.TempFolder = Path.Combine(this.RootPath, "Temp");
      this.InstanceDatabases = instance.AttachedDatabases;
      this.instanceName = instance.Name;
      this.StopInstance = instance.Stop;
      this.WebsiteID = instance.ID;
    }

    #endregion

    #region Properties

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
  }
}
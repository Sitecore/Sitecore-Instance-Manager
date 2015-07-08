#region Usings

using System.Data.SqlClient;
using System.IO;
using SIM.Adapters.SqlServer;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using SIM.Products;

#endregion

namespace SIM.Tool.Base.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   Defines the install wizard args class.
  /// </summary>
  public class InstallWizardArgs : InstallModulesWizardArgs
  {
    #region Properties

    /// <summary>
    ///   Gets or sets the instance app pool info.
    /// </summary>
    /// <value> The instance app pool info. </value>
    public AppPoolInfo InstanceAppPoolInfo { get; set; }

    /// <summary>
    ///   Gets or sets the instance connection string.
    /// </summary>
    /// <value> The instance connection string. </value>
    public SqlConnectionStringBuilder InstanceConnectionString { get; set; }

    /// <summary>
    ///   Gets or sets the instance host.
    /// </summary>
    /// <value> The instance host. </value>
    public string InstanceHost { get; set; }

    /// <summary>
    ///   Gets the name of the instance.
    /// </summary>
    /// <value> The name of the instance. </value>
    public new string InstanceName { get; set; }

    /// <summary>
    ///   Gets or sets the instance product.
    /// </summary>
    /// <value> The instance product. </value>
    public Product InstanceProduct { get; set; }

    /// <summary>
    ///   Gets or sets the instance root path.
    /// </summary>
    /// <value> The instance root path. </value>
    public string InstanceRootPath { get; set; }

    /// <summary>
    ///   Gets or sets the instance web root path.
    /// </summary>
    /// <value> The instance web root path. </value>
    public string InstanceWebRootPath { get; set; }

    /// <summary>
    ///   Gets or sets the license file info.
    /// </summary>
    /// <value> The license file info. </value>
    public FileInfo LicenseFileInfo { get; set; }

    /// <summary>
    ///   Gets or sets the product.
    /// </summary>
    /// <value> The product. </value>
    public override Product Product { get; set; }

    public new Instance Instance
    {
      get { return InstanceManager.GetInstance(this.InstanceName); }
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Converts the <see cref="InstallModulesWizardArgs" /> to a <see cref="ProcessorArgs" /> .
    /// </summary>
    /// <returns> The <see cref="ProcessorArgs" /> . </returns>
    public override ProcessorArgs ToProcessorArgs()
    {
      return new InstallArgs(this.InstanceName, this.InstanceHost, this.InstanceProduct, this.InstanceRootPath, this.InstanceConnectionString, SqlServerManager.Instance.GetSqlServerAccountName(this.InstanceConnectionString), Settings.CoreInstallWebServerIdentity.Value, this.LicenseFileInfo, this.InstanceAppPoolInfo.FrameworkVersion == "v4.0", this.InstanceAppPoolInfo.Enable32BitAppOnWin64, !this.InstanceAppPoolInfo.ManagedPipelineMode, this.Modules);
    }

    #endregion

    #region Public properties

    /// <summary>
    ///   Gets or sets the name of the instance root.
    /// </summary>
    /// <value> The name of the instance root. </value>
    public string InstanceRootName { get; set; }

    #endregion
  }
}
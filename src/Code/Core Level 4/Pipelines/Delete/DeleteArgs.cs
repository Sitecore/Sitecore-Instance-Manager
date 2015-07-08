#region Usings

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Delete
{
  


  #region

  using SIM.Adapters.MongoDb;

  #endregion

  /// <summary>
  ///   The delete args.
  /// </summary>
  public class DeleteArgs : ProcessorArgs
  {
    #region Fields

    /// <summary>
    ///   The connection string.
    /// </summary>
    [CanBeNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    public readonly Instance Instance;

    /// <summary>
    ///   The instance databases.
    /// </summary>
    public readonly IEnumerable<Database> InstanceDatabases;

    /// <summary>
    ///   The instance id.
    /// </summary>
    public readonly long InstanceID;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteArgs"/> class.
    /// </summary>
    /// <param name="instance">
    /// The instance. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    public DeleteArgs([NotNull] Instance instance, [NotNull] SqlConnectionStringBuilder connectionString) 
    {
      Instance = instance;
      Assert.ArgumentNotNull(instance, "instance");

      this.ConnectionString = connectionString.IsNotNull("ConnectionString");
      this.InstanceID = instance.ID;
      this.InstanceDatabases = instance.AttachedDatabases;
      this.MongoDatabases = instance.MongoDatabases;
      this.InstanceDataFolderPath = instance.DataFolderPath;
      this.InstanceBackupsFolder = instance.BackupsFolder;
      this.InstanceStop = () => instance.Stop(true);
      this.InstanceHostNames = instance.HostNames;
      this.instanceName = instance.Name;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets GetRootPath.
    /// </summary>
    [NotNull]
    public string RootPath { get; set; }

    /// <summary>
    ///   Gets or sets WebRootPath.
    /// </summary>
    [NotNull]
    public string WebRootPath { get; set; }

    /// <summary>
    /// Gets or sets the instance data folder path.
    /// </summary>
    public string InstanceDataFolderPath { get; set; }

    /// <summary>
    /// Gets or sets the instance backups folder.
    /// </summary>
    public string InstanceBackupsFolder { get; set; }

    #endregion

    private readonly string instanceName;
    public readonly ICollection<MongoDbDatabase> MongoDatabases;

    /// <summary>
    /// Gets or sets the instance stop method.
    /// </summary>
    public Action InstanceStop { get; set; }

    /// <summary>
    /// Gets or sets the instance host names.
    /// </summary>
    public IEnumerable<string> InstanceHostNames { get; set; }

    public string InstanceName
    {
      get { return this.instanceName; }
    }
  }
}
namespace SIM.Pipelines.Delete
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class DeleteArgs : ProcessorArgs
  {
    #region Fields

    [CanBeNull]
    public readonly SqlConnectionStringBuilder ConnectionString;

    public readonly Instance Instance;

    public readonly IEnumerable<Database> InstanceDatabases;

    public readonly long InstanceID;

    public readonly ICollection<MongoDbDatabase> MongoDatabases;
    private readonly string instanceName;

    #endregion

    #region Constructors

    public DeleteArgs([NotNull] Instance instance, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      this.Instance = instance;
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

    public string InstanceBackupsFolder { get; set; }
    public string InstanceDataFolderPath { get; set; }

    [NotNull]
    public string RootPath { get; set; }

    [NotNull]
    public string WebRootPath { get; set; }

    #endregion

    #region Public properties

    public IEnumerable<string> InstanceHostNames { get; set; }

    public string InstanceName
    {
      get
      {
        return this.instanceName;
      }
    }

    public Action InstanceStop { get; set; }

    #endregion
  }
}
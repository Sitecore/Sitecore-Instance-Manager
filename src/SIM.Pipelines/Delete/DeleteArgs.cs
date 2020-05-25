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
  using JetBrains.Annotations;

  #endregion

  public class DeleteArgs : ProcessorArgs
  {
    #region Fields

    [CanBeNull]
    public SqlConnectionStringBuilder ConnectionString { get; }

    public Instance Instance { get; }

    #endregion

    #region Constructors

    public DeleteArgs([NotNull] Instance instance, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Instance = instance;
      InstanceName = Instance.Name;
      Assert.ArgumentNotNull(instance, nameof(instance));
      ConnectionString = connectionString.IsNotNull("ConnectionString");
    }

    public void Initialize()
    {
      InstanceID = Instance.ID;
      InstanceDatabases = Instance.AttachedDatabases;
      MongoDatabases = Instance.MongoDatabases;
      InstanceDataFolderPath = Instance.DataFolderPath;
      InstanceBackupsFolder = Instance.BackupsFolder;
      InstanceStop = () => Instance.Stop(true);
      InstanceHostNames = Instance.HostNames;
      WebRootPath = Instance.WebRootPath;
      RootPath = Instance.RootPath;
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

    public string InstanceName { get; private set; }

    public Action InstanceStop { get; set; }

    public IEnumerable<Database> InstanceDatabases { get; private set; }

    public long InstanceID { get; private set; }

    public ICollection<MongoDbDatabase> MongoDatabases { get; private set; }

    //Indicates if the installation has been completed
    public bool HasInstallationBeenCompleted { get; set; }

    #endregion
  }
}
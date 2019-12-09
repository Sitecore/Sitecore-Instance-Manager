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

    public IEnumerable<Database> _InstanceDatabases { get; private set; }

    public long InstanceID { get; private set; }

    public ICollection<MongoDbDatabase> _MongoDatabases { get; private set; }
    private string instanceName { get; set; }

    #endregion

    #region Constructors

    public DeleteArgs([NotNull] Instance instance, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Instance = instance;
      Assert.ArgumentNotNull(instance, nameof(instance));
      ConnectionString = connectionString.IsNotNull("ConnectionString");
    }

    public void Initialize()
    {
      InstanceID = Instance.ID;
      _InstanceDatabases = Instance.AttachedDatabases;
      _MongoDatabases = Instance.MongoDatabases;
      InstanceDataFolderPath = Instance.DataFolderPath;
      InstanceBackupsFolder = Instance.BackupsFolder;
      InstanceStop = () => Instance.Stop(true);
      InstanceHostNames = Instance.HostNames;
      instanceName = Instance.Name;
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

    public string InstanceName
    {
      get
      {
        return instanceName;
      }
    }

    public Action InstanceStop { get; set; }

    #endregion
  }
}
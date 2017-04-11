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
  using SIM.Extensions;

  #endregion

  public class DeleteArgs : ProcessorArgs
  {
    #region Fields

    [CanBeNull]
    public SqlConnectionStringBuilder ConnectionString { get; }

    public Instance Instance { get; }

    public readonly IEnumerable<Database> _InstanceDatabases;

    public long InstanceID { get; }

    public readonly ICollection<MongoDbDatabase> _MongoDatabases;
    private string instanceName { get; }

    #endregion

    #region Constructors

    public DeleteArgs([NotNull] Instance instance, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Instance = instance;
      Assert.ArgumentNotNull(instance, nameof(instance));

      ConnectionString = connectionString.IsNotNull("ConnectionString");
      InstanceID = instance.ID;
      _InstanceDatabases = instance.AttachedDatabases;
      _MongoDatabases = instance.MongoDatabases;
      InstanceDataFolderPath = instance.DataFolderPath;
      InstanceBackupsFolder = instance.BackupsFolder;
      InstanceStop = () => instance.Stop(true);
      InstanceHostNames = instance.HostNames;
      instanceName = instance.Name;
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
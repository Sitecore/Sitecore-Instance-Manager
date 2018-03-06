namespace SIM.Pipelines.Backup
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;
  using System.Data.SqlClient;

  public class BackupArgs : ProcessorArgs
  {
    #region Fields

    public bool BackupClient { get; }

    public bool BackupDatabases { get; }

    public bool BackupFiles { get; }
    public bool BackupMongoDatabases { get; }

    public string BackupName { get; }

    [NotNull]
    public string Folder { get; }

    public Instance Instance { get; }
    public string _WebRootPath;
    public SqlConnectionStringBuilder ConnectionString { get; }

    private string _instanceName { get; }

    #endregion

    #region Constructors

    public BackupArgs([NotNull] Instance instance, SqlConnectionStringBuilder connectionString, string backupName = null, bool backupFiles = false, bool backupDatabases = false, bool backupClient = false, bool backupMongoDatabases = false)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      BackupFiles = backupFiles;
      BackupClient = backupClient;
      BackupMongoDatabases = backupMongoDatabases;
      BackupDatabases = backupDatabases;
      Instance = instance;
      _WebRootPath = instance.WebRootPath;
      BackupName = backupName;
      Folder = BackupName != null
        ? FileSystem.FileSystem.Local.Directory.Ensure(instance.GetBackupFolder(BackupName))
        : string.Empty;
      _instanceName = Instance.Name;
      ConnectionString = connectionString;
    }

    #endregion

    #region Public properties

    public string InstanceName
    {
      get
      {
        return _instanceName;
      }
    }

    #endregion
  }
}
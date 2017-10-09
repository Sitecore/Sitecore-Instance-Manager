namespace SIM.Pipelines.Backup
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

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
    private string _instanceName { get; }

    #endregion

    #region Constructors

    public BackupArgs([NotNull] Instance instance, string backupName = null, bool backupFiles = false, bool backupDatabases = false, bool backupClient = false, bool backupMongoDatabases = false)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
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
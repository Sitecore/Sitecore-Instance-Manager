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
      this.BackupFiles = backupFiles;
      this.BackupClient = backupClient;
      this.BackupMongoDatabases = backupMongoDatabases;
      this.BackupDatabases = backupDatabases;
      this.Instance = instance;
      this._WebRootPath = instance.WebRootPath;
      this.BackupName = backupName;
      this.Folder = this.BackupName != null
        ? FileSystem.FileSystem.Local.Directory.Ensure(instance.GetBackupFolder(this.BackupName))
        : string.Empty;
      this._instanceName = this.Instance.Name;
    }

    #endregion

    #region Public properties

    public string InstanceName
    {
      get
      {
        return this._instanceName;
      }
    }

    #endregion
  }
}
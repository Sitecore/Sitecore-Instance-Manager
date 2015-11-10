namespace SIM.Pipelines.Backup
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class BackupArgs : ProcessorArgs
  {
    #region Fields

    public readonly bool BackupClient;

    public readonly bool BackupDatabases;

    public readonly bool BackupFiles;
    public readonly bool BackupMongoDatabases;

    public readonly string BackupName;

    [NotNull]
    public readonly string Folder;

    public readonly Instance Instance;
    public string WebRootPath;
    private readonly string _instanceName;

    #endregion

    #region Constructors

    public BackupArgs([NotNull] Instance instance, string backupName = null, bool backupFiles = false, bool backupDatabases = false, bool backupClient = false, bool backupMongoDatabases = false)
    {
      Assert.ArgumentNotNull(instance, "instance");
      this.BackupFiles = backupFiles;
      this.BackupClient = backupClient;
      this.BackupMongoDatabases = backupMongoDatabases;
      this.BackupDatabases = backupDatabases;
      this.Instance = instance;
      this.WebRootPath = instance.WebRootPath;
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
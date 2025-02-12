namespace SIM.Pipelines.Backup
{
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;
  using System.Data.SqlClient;
  using System.Collections.Generic;
  using System.Linq;

  public class Backup9Args : ProcessorArgs
  {
    #region Fields

    public bool BackupClient { get; }

    public bool BackupDatabases { get; }

    public bool BackupFiles { get; }

    public string BackupName { get; }

    [NotNull]
    public string Folder { get; }

    public Instance Instance { get; }
    private string _instanceName { get; }
    public string _WebRootPath;
    public SqlConnectionStringBuilder ConnectionString { get; }

    public readonly ICollection<string> _SelectedDatabases;

    #endregion

    #region Constructors

    public Backup9Args([NotNull] Instance instance, SqlConnectionStringBuilder connectionString,
      string backupName = null,
      bool backupFiles = false, bool backupClient = false,
      bool backupDatabases = false, IEnumerable<string> selectedDatabases = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      Instance = instance;
      _instanceName = Instance.Name;

      _WebRootPath = instance.WebRootPath;
      BackupName = backupName;
      Folder = BackupName != null
        ? FileSystem.FileSystem.Local.Directory.Ensure(instance.GetBackupFolder(BackupName))
        : string.Empty;

      BackupFiles = backupFiles;
      BackupClient = backupClient;

      ConnectionString = connectionString;
      BackupDatabases = backupDatabases;
      _SelectedDatabases = selectedDatabases.With(x => x.Select(y => y.ToLower()).ToArray());
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
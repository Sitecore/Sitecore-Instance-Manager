namespace SIM.Instances
{
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public class InstanceBackup
  {
    #region Fields

    public bool BackupDataFiles { get; }

    public bool BackupDatabases { get; }

    public bool BackupMongoDatabases { get; }

    public bool BackupWebsiteFiles { get; }

    public bool BackupWebsiteFilesNoClient { get; }

    public string Date { get; }

    public string FolderPath { get; }

    #endregion

    #region Constructors

    public InstanceBackup([NotNull] string date, [NotNull] string folder, bool backupWebsiteFiles, bool backupDataFiles, bool backupDatabases, bool backupWebsiteFilesNoClient, bool backupMongoDatabases)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      BackupWebsiteFiles = backupWebsiteFiles;
      BackupWebsiteFilesNoClient = backupWebsiteFilesNoClient;
      BackupMongoDatabases = backupMongoDatabases;
      BackupDataFiles = backupDataFiles;
      BackupDatabases = backupDatabases;
      Date = date;
      FolderPath = folder;
    }

    public InstanceBackup(string date, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      FolderPath = folder;
      bool full = FileSystem.FileSystem.Local.File.Exists(WebRootFilePath);
      bool noclient = FileSystem.FileSystem.Local.File.Exists(WebRootNoClientFilePath);
      BackupWebsiteFiles = full || noclient;
      BackupWebsiteFilesNoClient = noclient;
      BackupDataFiles = FileSystem.FileSystem.Local.File.Exists(DataFilePath);

      BackupDatabases = FileSystem.FileSystem.Local.Directory.Exists(DatabasesFolderPath) && FileSystem.FileSystem.Local.Directory.GetFiles(DatabasesFolderPath, "*.bak").Length > 0;
      BackupMongoDatabases = FileSystem.FileSystem.Local.Directory.Exists(MongoDatabasesFolderPath) && FileSystem.FileSystem.Local.Directory.GetDirectories(MongoDatabasesFolderPath).Length > 0;
      Date = date;
    }

    #endregion

    #region Properties

    [NotNull]
    public string DataFilePath
    {
      get
      {
        return Path.Combine(FolderPath, "Data.zip");
      }
    }

    [NotNull]
    public string[] DatabaseFilenames
    {
      get
      {
        return FileSystem.FileSystem.Local.Directory.GetFiles(DatabasesFolderPath, '*' + SqlServerManager.BackupExtension);
      }
    }

    [NotNull]
    public string DatabasesFolderPath
    {
      get
      {
        return Path.Combine(FolderPath, "Databases");
      }
    }

    public string DatabasesString
    {
      get
      {
        const string Separator = ", ";
        var dbs = DatabaseFilenames.Aggregate(string.Empty, (current, file) => current + (Path.GetFileNameWithoutExtension(file) + Separator));
        dbs = dbs.Substring(0, dbs.Length - Separator.Length);
        return dbs;
      }
    }

    public string DateString
    {
      get
      {
        return Date; // .ToString("yyyy.MM.dd") + " at " + this.Date.ToString("hh:mm");
      }
    }

    [NotNull]
    public string[] MongoDatabaseFilenames
    {
      get
      {
        return FileSystem.FileSystem.Local.Directory.GetDirectories(MongoDatabasesFolderPath);
      }
    }

    [NotNull]
    public string MongoDatabasesFolderPath
    {
      get
      {
        return Path.Combine(FolderPath, "MongoDatabases");
      }
    }

    public string MongoDatabasesString
    {
      get
      {
        const string Separator = ", ";
        var dbs = MongoDatabaseFilenames.Aggregate(string.Empty, (current, file) => current + (Path.GetFileName(file) + Separator));
        dbs = dbs.Substring(0, dbs.Length - Separator.Length);
        return dbs;
      }
    }

    [NotNull]
    public string WebRootFilePath
    {
      get
      {
        return Path.Combine(FolderPath, "WebRoot.zip");
      }
    }

    [NotNull]
    public string WebRootNoClientFilePath
    {
      get
      {
        return Path.Combine(FolderPath, "WebRootNoClient.zip");
      }
    }

    #endregion

    #region Public Methods

    public override string ToString()
    {
      var dbs = string.Empty;
      if (BackupDatabases)
      {
        dbs = DatabasesString;
      }

      if (BackupMongoDatabases)
      {
        dbs += MongoDatabasesString;
      }

      var backupDbs = BackupDatabases || BackupMongoDatabases;
      var files = BackupWebsiteFiles ? (BackupWebsiteFilesNoClient ? "files (no client)" : "files") : string.Empty;
      var and = BackupWebsiteFiles && backupDbs ? " and " : string.Empty;
      var databases = backupDbs ? $"databases: {dbs}" : string.Empty;
      return
        $"{DateString}: {files}{and}{databases}";
    }

    #endregion
  }
}
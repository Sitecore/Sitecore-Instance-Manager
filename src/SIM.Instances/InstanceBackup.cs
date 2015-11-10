namespace SIM.Instances
{
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public class InstanceBackup
  {
    #region Fields

    public readonly bool BackupDataFiles;

    public readonly bool BackupDatabases;

    public readonly bool BackupMongoDatabases;

    public readonly bool BackupWebsiteFiles;

    public readonly bool BackupWebsiteFilesNoClient;

    public readonly string Date;

    public readonly string FolderPath;

    #endregion

    #region Constructors

    public InstanceBackup([NotNull] string date, [NotNull] string folder, bool backupWebsiteFiles, bool backupDataFiles, bool backupDatabases, bool backupWebsiteFilesNoClient, bool backupMongoDatabases)
    {
      Assert.ArgumentNotNull(folder, "folder");

      this.BackupWebsiteFiles = backupWebsiteFiles;
      this.BackupWebsiteFilesNoClient = backupWebsiteFilesNoClient;
      this.BackupMongoDatabases = backupMongoDatabases;
      this.BackupDataFiles = backupDataFiles;
      this.BackupDatabases = backupDatabases;
      this.Date = date;
      this.FolderPath = folder;
    }

    public InstanceBackup(string date, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, "folder");

      this.FolderPath = folder;
      bool full = FileSystem.FileSystem.Local.File.Exists(this.WebRootFilePath);
      bool noclient = FileSystem.FileSystem.Local.File.Exists(this.WebRootNoClientFilePath);
      this.BackupWebsiteFiles = full || noclient;
      this.BackupWebsiteFilesNoClient = noclient;
      this.BackupDataFiles = FileSystem.FileSystem.Local.File.Exists(this.DataFilePath);

      this.BackupDatabases = FileSystem.FileSystem.Local.Directory.Exists(this.DatabasesFolderPath) && FileSystem.FileSystem.Local.Directory.GetFiles(this.DatabasesFolderPath, "*.bak").Length > 0;
      this.BackupMongoDatabases = FileSystem.FileSystem.Local.Directory.Exists(this.MongoDatabasesFolderPath) && FileSystem.FileSystem.Local.Directory.GetDirectories(this.MongoDatabasesFolderPath).Length > 0;
      this.Date = date;
    }

    #endregion

    #region Properties

    [NotNull]
    public string DataFilePath
    {
      get
      {
        return Path.Combine(this.FolderPath, "Data.zip");
      }
    }

    [NotNull]
    public string[] DatabaseFilenames
    {
      get
      {
        return FileSystem.FileSystem.Local.Directory.GetFiles(this.DatabasesFolderPath, '*' + SqlServerManager.BackupExtension);
      }
    }

    [NotNull]
    public string DatabasesFolderPath
    {
      get
      {
        return Path.Combine(this.FolderPath, "Databases");
      }
    }

    public string DatabasesString
    {
      get
      {
        const string Separator = ", ";
        string dbs = this.DatabaseFilenames.Aggregate(string.Empty, (current, file) => current + (Path.GetFileNameWithoutExtension(file) + Separator));
        dbs = dbs.Substring(0, dbs.Length - Separator.Length);
        return dbs;
      }
    }

    public string DateString
    {
      get
      {
        return this.Date; // .ToString("yyyy.MM.dd") + " at " + this.Date.ToString("hh:mm");
      }
    }

    [NotNull]
    public string[] MongoDatabaseFilenames
    {
      get
      {
        return FileSystem.FileSystem.Local.Directory.GetDirectories(this.MongoDatabasesFolderPath);
      }
    }

    [NotNull]
    public string MongoDatabasesFolderPath
    {
      get
      {
        return Path.Combine(this.FolderPath, "MongoDatabases");
      }
    }

    public string MongoDatabasesString
    {
      get
      {
        const string Separator = ", ";
        string dbs = this.MongoDatabaseFilenames.Aggregate(string.Empty, (current, file) => current + (Path.GetFileName(file) + Separator));
        dbs = dbs.Substring(0, dbs.Length - Separator.Length);
        return dbs;
      }
    }

    [NotNull]
    public string WebRootFilePath
    {
      get
      {
        return Path.Combine(this.FolderPath, "WebRoot.zip");
      }
    }

    [NotNull]
    public string WebRootNoClientFilePath
    {
      get
      {
        return Path.Combine(this.FolderPath, "WebRootNoClient.zip");
      }
    }

    #endregion

    #region Public Methods

    public override string ToString()
    {
      string dbs = string.Empty;
      if (this.BackupDatabases)
      {
        dbs = this.DatabasesString;
      }

      if (this.BackupMongoDatabases)
      {
        dbs += this.MongoDatabasesString;
      }

      var backupDbs = this.BackupDatabases || this.BackupMongoDatabases;
      return this.DateString + ": " + (this.BackupWebsiteFiles ? (this.BackupWebsiteFilesNoClient ? "files (no client)" : "files") : string.Empty) + (this.BackupWebsiteFiles && backupDbs ? " and " : string.Empty) + (backupDbs ? "databases: " + dbs : string.Empty);
    }

    #endregion
  }
}
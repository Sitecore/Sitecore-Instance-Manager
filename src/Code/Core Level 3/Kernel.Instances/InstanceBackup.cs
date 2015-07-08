#region Usings

using System;
using System.IO;
using System.Linq;
using SIM.Adapters.SqlServer;
using SIM.Base;

#endregion

namespace SIM.Instances
{
  #region

  

  #endregion

  /// <summary>
  ///   The instance backup.
  /// </summary>
  public class InstanceBackup
  {
    #region Fields

    /// <summary>
    ///   The backup data files.
    /// </summary>
    public readonly bool BackupDataFiles;

    /// <summary>
    ///   The backup databases.
    /// </summary>
    public readonly bool BackupDatabases;

    public readonly bool BackupMongoDatabases;

    /// <summary>
    ///   The backup website files.
    /// </summary>
    public readonly bool BackupWebsiteFiles;

    /// <summary>
    ///   The backup website files.
    /// </summary>
    public readonly bool BackupWebsiteFilesNoClient;

    /// <summary>
    ///   The date.
    /// </summary>
    public readonly string Date;

    /// <summary>
    ///   The folder path.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceBackup"/> class.
    /// </summary>
    /// <param name="date">
    /// The date. 
    /// </param>
    /// <param name="folder">
    /// The folder. 
    /// </param>
    public InstanceBackup(string date, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, "folder");

      this.FolderPath = folder;
      bool full = FileSystem.Local.File.Exists(this.WebRootFilePath);
      bool noclient = FileSystem.Local.File.Exists(this.WebRootNoClientFilePath);
      this.BackupWebsiteFiles = full || noclient;
      this.BackupWebsiteFilesNoClient = noclient;
      this.BackupDataFiles = FileSystem.Local.File.Exists(this.DataFilePath);

      this.BackupDatabases = FileSystem.Local.Directory.Exists(this.DatabasesFolderPath) && FileSystem.Local.Directory.GetFiles(this.DatabasesFolderPath, "*.bak").Length > 0;
      this.BackupMongoDatabases = FileSystem.Local.Directory.Exists(this.MongoDatabasesFolderPath) && FileSystem.Local.Directory.GetDirectories(this.MongoDatabasesFolderPath).Length > 0;
      this.Date = date;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets DataFilePath.
    /// </summary>
    [NotNull]
    public string DataFilePath
    {
      get
      {
        return Path.Combine(this.FolderPath, "Data.zip");
      }
    }

    /// <summary>
    ///   Gets DatabaseFilenames.
    /// </summary>
    [NotNull]
    public string[] DatabaseFilenames
    {
      get
      {
        return FileSystem.Local.Directory.GetFiles(this.DatabasesFolderPath, '*' + SqlServerManager.BackupExtension);
      }
    }

    [NotNull]
    public string[] MongoDatabaseFilenames
    {
      get
      {
        return FileSystem.Local.Directory.GetDirectories(this.MongoDatabasesFolderPath);
      }
    }

    /// <summary>
    ///   Gets DatabasesFolderPath.
    /// </summary>
    [NotNull]
    public string DatabasesFolderPath
    {
      get
      {
        return Path.Combine(this.FolderPath, "Databases");
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

    /// <summary>
    ///   Gets DatabasesString.
    /// </summary>
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

    /// <summary>
    ///   Gets DateString.
    /// </summary>
    public string DateString
    {
      get
      {
        return this.Date;//.ToString("yyyy.MM.dd") + " at " + this.Date.ToString("hh:mm");
      }
    }

    /// <summary>
    ///   Gets WebRootFilePath.
    /// </summary>
    [NotNull]
    public string WebRootFilePath
    {
      get
      {
        return Path.Combine(this.FolderPath, "WebRoot.zip");
      }
    }

    /// <summary>
    ///   Gets WebRootFilePathWithoutClient.
    /// </summary>
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

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The to string. </returns>
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
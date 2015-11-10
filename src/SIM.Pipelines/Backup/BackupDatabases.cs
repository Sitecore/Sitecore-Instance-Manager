namespace SIM.Pipelines.Backup
{
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  [UsedImplicitly]
  public class BackupDatabases : BackupProcessor
  {
    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.AttachedDatabases.Count();
    }

    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.BackupDatabases;
    }

    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      Assert.IsNotNull(instance, "instance");
      var backupDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "Databases"));

      foreach (Database database in instance.AttachedDatabases)
      {
        this.Backup(database, backupDatabasesFolder);
        this.IncrementProgress();
      }
    }

    #endregion

    #region Private methods

    private void Backup([NotNull] Database database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(folder, "folder");

      var connectionString = database.ConnectionString;

      using (var connection = SqlServerManager.Instance.OpenConnection(connectionString))
      {
        var databaseName = database.RealName;
        var fileName = Path.Combine(folder, database.BackupFilename);
        Log.Info("Backing up the '{0}' database to the '{1}' file", databaseName, fileName);

        var command = "BACKUP DATABASE [" + databaseName + "] TO  DISK = N'" + fileName +
                      "' WITH NOFORMAT, NOINIT,  NAME = N'" + databaseName +
                      " initial backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
        SqlServerManager.Instance.Execute(connection, command);
      }
    }

    #endregion

    #endregion
  }
}
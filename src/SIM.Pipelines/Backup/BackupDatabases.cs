namespace SIM.Pipelines.Backup
{
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using System.Data.SqlClient;

  [UsedImplicitly]
  public class BackupDatabases : BackupProcessor
  {
    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args.Instance.AttachedDatabases.Count();
    }

    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args.BackupDatabases;
    }

    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));
      var backupDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "Databases"));

      foreach (Database database in instance.AttachedDatabases)
      {
        Backup(database, backupDatabasesFolder, args.ConnectionString);
        IncrementProgress();
      }
    }

    #endregion

    #region Private methods

    private void Backup([NotNull] Database database, [NotNull] string folder, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(folder, nameof(folder));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      using (var connection = SqlServerManager.Instance.OpenConnection(connectionString, true))
      {
        var databaseName = database.RealName;
        var fileName = Path.Combine(folder, database.BackupFilename);
        Log.Info($"Backing up the '{databaseName}' database to the '{fileName}' file");

        var command = $"BACKUP DATABASE [{databaseName}] TO  " +
                      $"DISK = N\'{fileName}\' WITH NOFORMAT, NOINIT,  " +
                      $"NAME = N\'{databaseName} initial backup\', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";

        SqlServerManager.Instance.Execute(connection, command);
      }
    }

    #endregion

    #endregion
  }
}
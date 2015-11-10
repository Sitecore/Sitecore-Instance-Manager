namespace SIM.Pipelines.Backup
{
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class BackupMongoDatabases : BackupProcessor
  {
    #region Methods

    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.MongoDatabases.Count();
    }

    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.BackupMongoDatabases;
    }

    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      Assert.IsNotNull(instance, "instance");

      var backupDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "MongoDatabases"));
      foreach (var database in instance.MongoDatabases)
      {
        MongoHelper.Backup(database, backupDatabasesFolder);
        this.IncrementProgress();
      }
    }

    #endregion
  }
}
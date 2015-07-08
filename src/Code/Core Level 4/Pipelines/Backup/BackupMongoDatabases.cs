namespace SIM.Pipelines.Backup
{
  using System;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Base;

  [UsedImplicitly]
  public class BackupMongoDatabases : BackupProcessor
  {
    #region Methods

    #region Protected methods

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    protected override long EvaluateStepsCount(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.MongoDatabases.Count();
    }

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected override bool IsRequireProcessing(BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.BackupMongoDatabases;
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] BackupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      Assert.IsNotNull(instance, "instance");

      var backupDatabasesFolder = FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "MongoDatabases"));
      foreach (var database in instance.MongoDatabases)
      {
        MongoHelper.Backup(database, backupDatabasesFolder);
        this.IncrementProgress();
      }
    }

    #endregion

    #endregion
  }
}
namespace SIM.Pipelines.Restore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Base;
  using SIM.Instances;

  [UsedImplicitly]
  public class RestoreMongoDatabases : RestoreProcessor
  {
    protected readonly List<string> done = new List<string>();

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
    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.MongoDatabaseFilenames.Count();
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
    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupMongoDatabases;
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var databases = args.Backup.MongoDatabaseFilenames;

      var instance = args.Instance;

      try
      {
        instance.Stop();
        foreach (var database in databases)
        {
          if (this.done.Contains(database))
          {
            continue;
          }

          MongoHelper.Restore(database);
          this.IncrementProgress();

          this.done.Add(database);
        }
      }
      finally
      {
        try
        {
          instance.Start();
        }
        catch (Exception ex)
        {
          Log.Warn("An error occurred during starting an instance", this, ex);
        }
      }


      this.IncrementProgress();
    }

    #endregion

    #endregion
  }
}
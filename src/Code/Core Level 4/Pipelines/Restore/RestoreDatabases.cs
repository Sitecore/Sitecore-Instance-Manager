#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;
using SIM.Adapters.SqlServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Restore
{
  #region

  

  #endregion

  /// <summary>
  ///   The restore databases.
  /// </summary>
  [UsedImplicitly]
  public class RestoreDatabases : RestoreProcessor
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

      return this.GetDatabases(args).Count();
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

      return args.Backup.BackupDatabases;
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

      IEnumerable<Database> databases = this.GetDatabases(args);

      var instance = args.Instance;
      var isStarted = instance.ApplicationPoolState == ObjectState.Started;
      instance.StopApplicationPool();
      try
      {
        foreach (Database database in databases)
        {
          var value = database.Name;
          if (done.Contains(value))
          {
            continue;
          }

          string bak = Path.Combine(args.Backup.DatabasesFolderPath, database.BackupFilename);
          database.Restore(bak);
          this.IncrementProgress();

          done.Add(value);
        }
      }
      finally 
      {
        if (isStarted)
        {
          instance.Start();
        }
      }

      this.IncrementProgress();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// The get databases.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private IEnumerable<Database> GetDatabases([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Database[] dbs = args.Instance.AttachedDatabases.ToArray();
      foreach (string databaseFilename in args.Backup.DatabaseFilenames)
      {
        string databaseName = Path.GetFileNameWithoutExtension(databaseFilename);
        foreach (Database database in dbs)
        {
          if (database.Name.Equals(databaseName))
          {
            yield return database;
            break;
          }
        }
      }

      // return new List<Database>(args.Backup.DatabaseNames.Select(name => args.Instance.GetAttachedDatabases.SingleOrDefault(db => db.Name.EqualsIgnoreCase(name))));
    }

    #endregion

    #endregion
  }
}
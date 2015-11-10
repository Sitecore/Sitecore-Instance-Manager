namespace SIM.Pipelines.Restore
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Microsoft.Web.Administration;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class RestoreDatabases : RestoreProcessor
  {
    #region Fields

    protected readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.GetDatabases(args).Count();
    }

    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupDatabases;
    }

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
          if (this.done.Contains(value))
          {
            continue;
          }

          string bak = Path.Combine(args.Backup.DatabasesFolderPath, database.BackupFilename);
          database.Restore(bak);
          this.IncrementProgress();

          this.done.Add(value);
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
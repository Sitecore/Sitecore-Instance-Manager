namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Base;

  [UsedImplicitly]
  public class ExportMongoDatabases : ExportProcessor
  {
    private readonly List<string> done = new List<string>();

    #region Methods

    #region Protected methods

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected override bool IsRequireProcessing(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.IncludeMongoDatabases;
    }

    protected override long EvaluateStepsCount([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.MongoDatabases.Count();
    }

    protected override void Process(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var mongoDatabases = args.Instance.MongoDatabases;
      var exportDatabasesFolder = FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "MongoDatabases"));

      foreach (var database in mongoDatabases)
      {
        if (this.done.Contains(database.Name))
        {
          continue;
        }

        MongoHelper.Backup(database, exportDatabasesFolder);
        this.IncrementProgress();

        this.done.Add(database.Name);
      }
    }

    #endregion

    #endregion
  }
}
namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ExportMongoDatabases : ExportProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    protected override long EvaluateStepsCount([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.MongoDatabases.Count();
    }

    protected override bool IsRequireProcessing(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.IncludeMongoDatabases;
    }

    protected override void Process(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var mongoDatabases = args.Instance.MongoDatabases;
      var exportDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "MongoDatabases"));

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
  }
}
namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ExportDatabases : ExportProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Instance.AttachedDatabases.Count();
    }

    protected override void Process([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var selectedDatabases = args.SelectedDatabases;
      var attachedDatabases = args.Instance.AttachedDatabases;
      var exportDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "Databases"));

      foreach (var database in attachedDatabases.Where(database => selectedDatabases.Contains(database.Name.ToLower())))
      {
        if (this.done.Contains(database.Name))
        {
          continue;
        }

        this.Backup(database, exportDatabasesFolder);
        this.done.Add(database.Name);
      }
    }

    #endregion

    #region Private methods

    private void Backup([NotNull] Database database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(folder, "folder");

      var connectionString = database.ConnectionString;

      var databaseName = database.RealName;
      var fileName = Path.Combine(folder, database.BackupFilename);
      SqlServerManager.Instance.BackupDatabase(connectionString, databaseName, fileName);
      this.IncrementProgress();
    }

    #endregion

    #endregion
  }
}
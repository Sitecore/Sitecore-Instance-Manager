namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class ExportDatabases : ExportProcessor
  {
    #region Fields

    private readonly List<string> _Done = new List<string>();

    #endregion

    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return args.Instance.AttachedDatabases.Count();
    }

    protected override void Process([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var selectedDatabases = args._SelectedDatabases;
      var attachedDatabases = args.Instance.AttachedDatabases;
      var exportDatabasesFolder = FileSystem.FileSystem.Local.Directory.Ensure(Path.Combine(args.Folder, "Databases"));

      foreach (var database in attachedDatabases.Where(database => selectedDatabases.Contains(database.Name.ToLower())))
      {
        if (_Done.Contains(database.Name))
        {
          continue;
        }

        Backup(database, exportDatabasesFolder);
        _Done.Add(database.Name);
      }
    }

    #endregion

    #region Private methods

    private void Backup([NotNull] Database database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(folder, nameof(folder));

      var connectionString = database.ConnectionString;

      var databaseName = database.RealName;
      var fileName = Path.Combine(folder, database.BackupFilename);
      SqlServerManager.Instance.BackupDatabase(connectionString, databaseName, fileName);
      IncrementProgress();
    }

    #endregion

    #endregion
  }
}
namespace SIM.Pipelines.Delete
{
  using System.Collections.Generic;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDatabases : DeleteProcessor
  {
    #region Fields

    private readonly List<string> _Done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      IEnumerable<Database> detectedDatabases = args.InstanceDatabases;
      var rootPath = args.RootPath.ToLower();
      var connectionString = args.ConnectionString;
      var instanceName = args.InstanceName;
      IPipelineController controller = Controller;

      DeleteDatabasesHelper.Process(detectedDatabases, rootPath, connectionString, instanceName, controller, _Done);
    }

    #endregion
  }
}
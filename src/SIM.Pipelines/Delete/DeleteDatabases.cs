namespace SIM.Pipelines.Delete
{
  using System.Collections.Generic;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDatabases : DeleteProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      IEnumerable<Database> detectedDatabases = args.InstanceDatabases;
      string rootPath = args.RootPath.ToLower();
      var connectionString = args.ConnectionString;
      string instanceName = args.InstanceName;
      IPipelineController controller = this.Controller;

      DeleteDatabasesHelper.Process(detectedDatabases, rootPath, connectionString, instanceName, controller, this.done);
    }

    #endregion
  }
}
namespace SIM.Pipelines.Reinstall
{
  using System.Collections.Generic;
  using SIM.Adapters.SqlServer;
  using SIM.Pipelines.Delete;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDatabases : ReinstallProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
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
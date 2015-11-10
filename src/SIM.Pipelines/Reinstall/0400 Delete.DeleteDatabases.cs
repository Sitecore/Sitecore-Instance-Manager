namespace SIM.Pipelines.Reinstall
{
  using System.Collections.Generic;
  using SIM.Adapters.SqlServer;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class DeleteDatabases : ReinstallProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return ((ReinstallArgs)args).InstanceDatabases.Count;
    }

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      IEnumerable<Database> detectedDatabases = args.InstanceDatabases;
      string rootPath = args.RootPath.ToLower();
      var connectionString = args.ConnectionString;
      string instanceName = args.InstanceName;
      IPipelineController controller = this.Controller;

      DeleteDatabasesHelper.Process(detectedDatabases, rootPath, connectionString, instanceName, controller, this.done);

      if (controller != null)
      {
        controller.IncrementProgress(args.InstanceDatabases.Count - 1);
      }
    }

    #endregion
  }
}
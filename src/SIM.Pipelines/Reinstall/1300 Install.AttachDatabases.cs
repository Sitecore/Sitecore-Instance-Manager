namespace SIM.Pipelines.Reinstall
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class AttachDatabases : ReinstallProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();
    
    #endregion

    #region Methods

    public override long EvaluateStepsCount([CanBeNull] ProcessorArgs args)
    {
      return AttachDatabasesHelper.StepsCount;
    }

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var defaultConnectionString = args.ConnectionString;
      Assert.IsNotNull(defaultConnectionString, "SQL Connection String isn't set in the Settings dialog");

      var instanceName = args.instanceName;
      var instance = InstanceManager.GetInstance(instanceName);
      var controller = this.Controller;

      var sqlPrefix = AttachDatabasesHelper.GetSqlPrefix(instance);

      foreach (ConnectionString connectionString in instance.Configuration.ConnectionStrings)
      {
        if (this.done.Contains(connectionString.Name))
        {
          continue;
        }

        AttachDatabasesHelper.AttachDatabase(connectionString, defaultConnectionString, args.Name, sqlPrefix, true, args.DatabasesFolderPath, args.InstanceName, controller);

        if (controller != null)
        {
          controller.IncrementProgress(AttachDatabasesHelper.StepsCount / args.ConnectionString.Count);
        }
        
        this.done.Add(connectionString.Name);
      }
    }

    #endregion
  }
}
namespace SIM.Pipelines.Install
{
  using System.Collections.Generic;
  using SIM.Adapters.WebServer;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class AttachDatabases : InstallProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();
    
    #endregion

    #region Methods

    public override long EvaluateStepsCount([CanBeNull] ProcessorArgs args)
    {
      return AttachDatabasesHelper.StepsCount;
    }

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var defaultConnectionString = args.ConnectionString;
      Assert.IsNotNull(defaultConnectionString, "SQL Connection String isn't set in the Settings dialog");

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));

      var sqlPrefix = args.InstanceSqlPrefix;
      Assert.IsNotNull(sqlPrefix, nameof(sqlPrefix));

      var controller = this.Controller;

      foreach (ConnectionString connectionString in instance.Configuration.ConnectionStrings)
      {
        if (this.done.Contains(connectionString.Name))
        {
          continue;
        }

        AttachDatabasesHelper.AttachDatabase(connectionString, defaultConnectionString, args.Name, sqlPrefix, args.InstanceAttachSql, args.DatabasesFolderPath, instance.Name, controller);

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
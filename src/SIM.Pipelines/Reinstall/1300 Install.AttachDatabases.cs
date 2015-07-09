namespace SIM.Pipelines.Reinstall
{
  using System.Collections.Generic;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class AttachDatabases : ReinstallProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var defaultConnectionString = args.ConnectionString;
      Assert.IsNotNull(defaultConnectionString, "SQL Connection String isn't set in the Settings dialog");

      string instanceName = args.instanceName;
      var instance = InstanceManager.GetInstance(instanceName);
      foreach (ConnectionString connectionString in instance.Configuration.ConnectionStrings)
      {
        if (this.done.Contains(connectionString.Name))
        {
          continue;
        }

        AttachDatabasesHelper.AttachDatabase(connectionString, defaultConnectionString, args.Name, args.DatabasesFolderPath, args.InstanceName, this.Controller);

        this.done.Add(connectionString.Name);
      }
    }

    #endregion
  }
}
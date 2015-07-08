#region Usings

using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Install;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The attach databases.
  /// </summary>
  [UsedImplicitly]
  public class AttachDatabases : ReinstallProcessor
  {
    #region Fields

    /// <summary>
    ///   The done.
    /// </summary>
    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    #region Protected methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <exception cref="SqlException">
    /// <c>SqlException</c>
    ///   .
    /// </exception>
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

    #endregion
  }
}
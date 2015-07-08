#region Usings

using System;
using System.Collections.Generic;
using SIM.Adapters.SqlServer;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete databases.
  /// </summary>
  [UsedImplicitly]
  public class DeleteDatabases : DeleteProcessor
  {
    private readonly List<string> done = new List<string>();

    #region Methods

    #region Protected methods

    /// <summary>
    /// Processes the processor.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Can't detect valid connection string
    /// </exception>
    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      IEnumerable<Database> detectedDatabases = args.InstanceDatabases;
      string rootPath = args.RootPath.ToLower();
      var connectionString = args.ConnectionString;
      string instanceName = args.InstanceName;
      IPipelineController controller = this.Controller;

      DeleteDatabasesHelper.Process(detectedDatabases, rootPath, connectionString, instanceName, controller, done);
    }

    #endregion

    #endregion
  }
}
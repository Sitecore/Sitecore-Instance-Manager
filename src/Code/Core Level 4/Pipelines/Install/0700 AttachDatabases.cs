#region Usings

using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Adapters.WebServer;
using SIM.Base;
using System.Linq;

#endregion

namespace SIM.Pipelines.Install
{
  #region



  #endregion

  /// <summary>
  ///   The attach databases.
  /// </summary>
  [UsedImplicitly]
  public class AttachDatabases : InstallProcessor
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
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var defaultConnectionString = args.ConnectionString;
      Assert.IsNotNull(defaultConnectionString, "SQL Connection String isn't set in the Settings dialog");

      var instance = args.Instance;
      Assert.IsNotNull(instance, "instance");

      foreach (ConnectionString connectionString in instance.Configuration.ConnectionStrings)
      {
        if (this.done.Contains(connectionString.Name))
        {
          continue;
        }

        AttachDatabasesHelper.AttachDatabase(connectionString, defaultConnectionString, args.Name, args.DatabasesFolderPath, instance.Name, this.Controller);

        this.done.Add(connectionString.Name);
      }
    }

    #endregion

    #endregion
  }
}
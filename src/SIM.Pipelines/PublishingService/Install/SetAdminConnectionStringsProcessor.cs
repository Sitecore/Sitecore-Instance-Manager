using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIM.Pipelines.PublishingService.Install
{
  public class SetAdminConnectionStringsProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    //Using the admin credentials for connection strings is necessary to upgrade and reset the database schema
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      foreach (KeyValuePair<string, SqlConnectionStringBuilder> connString in args.SPSConnectionStrings)
      {
        string AdminConnString = new SqlConnectionStringBuilder(connString.Value.ToString())
        {
          UserID = args.SqlAdminUsername,
          Password = args.SqlAdminPassword
        }.ToString();

        Commands.SetConnectionString(connString.Key, AdminConnString);
      }
    }
  }
}

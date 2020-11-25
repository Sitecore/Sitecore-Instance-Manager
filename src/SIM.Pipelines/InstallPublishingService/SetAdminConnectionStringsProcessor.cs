using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class SetAdminConnectionStringsProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    //Using the admin credentials for connection strings is necessary to upgrade and reset the database schema
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.SPSWebroot);
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

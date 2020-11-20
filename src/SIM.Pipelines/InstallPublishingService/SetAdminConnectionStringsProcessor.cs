using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class SetAdminConnectionStringsProcessor : InstallPublishingServiceProcessor
  {
    //Using the admin credentials for connection strings is necessary to upgrade and reset the database schema
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.PubilshingServiceWebroot);
      foreach (KeyValuePair<string, SqlConnectionStringBuilder> connString in args.PublishingServiceConnectionStrings)
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

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class SetActualConnectionStringsProcessor : InstallPublishingServiceProcessor
  {
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.PublishingServiceWebroot);
      foreach (KeyValuePair<string, SqlConnectionStringBuilder> connString in args.PublishingServiceConnectionStrings)
      {
        Commands.SetConnectionString(connString.Key, connString.Value.ToString());
      }
    }
  }
}

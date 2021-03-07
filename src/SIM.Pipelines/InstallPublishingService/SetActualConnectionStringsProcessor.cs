using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace SIM.Pipelines.InstallPublishingService
{
  public class SetActualConnectionStringsProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      Directory.SetCurrentDirectory(args.SPSWebroot);
      foreach (KeyValuePair<string, SqlConnectionStringBuilder> connString in args.SPSConnectionStrings)
      {
        Commands.SetConnectionString(connString.Key, connString.Value.ToString());
      }
    }
  }
}

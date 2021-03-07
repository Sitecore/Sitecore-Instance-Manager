using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics.Logging;
using System.Diagnostics;
using System.Net;

namespace SIM.Pipelines.InstallPublishingService
{
  public class VerifyInstallationProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      string statusEndpoint = $"http://{args.SPSSiteName}:{args.SPSPort}/api/publishing/operations/status";
      if (!ValidateStatusEndpoint(statusEndpoint))
      {
        Log.Error("Publishing Service returned errors");
      }
      OpenStatusEndpoint(statusEndpoint);
    }

    private bool ValidateStatusEndpoint(string statusEndpoint)
    {
      Log.Info($"HTTP GET {statusEndpoint}");
      JObject response;
      using (WebClient wc = new WebClient())
      {
        response = JObject.Parse(wc.DownloadString(statusEndpoint));
      }

      return response["status"].Value<int>().Equals(0);
    }

    private void OpenStatusEndpoint(string statusEndpoint)
    {
      System.Diagnostics.Process.Start(new ProcessStartInfo(statusEndpoint));
    }
  }
}

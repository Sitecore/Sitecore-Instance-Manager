using System;
using System.Linq;
using SIM.Pipelines.InstallPublishingService;
using SIM.SitecoreEnvironments;
using Sitecore.Diagnostics.Logging;

namespace SIM.Pipelines.UninstallPublishingService
{
  public class RemoveFromEnvironmentProcessor : SPSProcessor<UninstallSPSProcessorArgs>
  {
    protected override void ProcessCore(UninstallSPSProcessorArgs args)
    {
      try
      {
        SitecoreEnvironmentMember SPSMember = args.Instance.SitecoreEnvironment.Members.FirstOrDefault(member => member.Name == args.SPSSiteName);
        if (SPSMember != null)
        {
          args.Instance.SitecoreEnvironment.Members.Remove(SPSMember);
        }
        SitecoreEnvironmentHelper.SaveSitecoreEnvironmentData(SitecoreEnvironmentHelper.SitecoreEnvironments);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Exception thrown while removing SPS instance from CM environment:\n{ex}");
      }
    }
  }
}

using System;
using System.Collections.Generic;
using SIM.SitecoreEnvironments;
using Sitecore.Diagnostics.Logging;

namespace SIM.Pipelines.InstallPublishingService
{
  public class AddToEnvironmentProcessor : SPSProcessor<InstallSPSProcessorArgs>
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      try
      {
        SitecoreEnvironment se = SitecoreEnvironmentHelper.GetExistingOrNewSitecoreEnvironment(args.CMInstance.Name);
        if (se.Members == null)
        {
          se.Members = new List<SitecoreEnvironmentMember>();
        }
        se.Members.Add(new SitecoreEnvironmentMember(args.SPSSiteName, SitecoreEnvironmentMember.Types.Site.ToString()));
        SitecoreEnvironmentHelper.AddSitecoreEnvironment(se);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Exception thrown while adding new SPS instance to CM environment:\n{ex}");
      }
    }
  }
}

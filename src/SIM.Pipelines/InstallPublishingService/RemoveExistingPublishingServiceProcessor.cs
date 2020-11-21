using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class RemoveExistingPublishingServiceProcessor : InstallPublishingServiceProcessor
  {
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      if (!args.OverwriteExisting)
      {
        return;
      }

      using (ServerManager IISManager = new ServerManager())
      {
        Site spsSite = IISManager.Sites[args.PublishingServiceSiteName];
        ApplicationPool spsAppPool = IISManager.ApplicationPools[args.PublishingServiceSiteName];

        if (spsSite != null)
        {
          spsSite.Stop();
          IISManager.Sites.Remove(spsSite);
        }

        if (spsAppPool != null)
        {
          spsAppPool.Stop();
          IISManager.ApplicationPools.Remove(spsAppPool);
        }

        IISManager.CommitChanges();
      }

      if (Directory.Exists(args.PublishingServiceWebroot))
      {
        Directory.Delete(args.PublishingServiceWebroot);
      }
    }
  }
}

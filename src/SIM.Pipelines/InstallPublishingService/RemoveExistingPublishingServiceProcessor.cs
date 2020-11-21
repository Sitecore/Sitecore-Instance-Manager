using JetBrains.Annotations;
using Microsoft.Web.Administration;
using SIM.Adapters.WebServer;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class RemoveExistingPublishingServiceProcessor : InstallPublishingServiceProcessor
  {
    private const int STOP_RETRIES = 6;
    private const int RETRY_INTERVAL_MS = 5000;

    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      if (!args.OverwriteExisting)
      {
        this.Skip();
        return;
      }

      using (ServerManager sm = new ServerManager())
      {
        Site spsSite = sm.Sites.FirstOrDefault(s => s.Name.Equals(args.PublishingServiceSiteName));
        ApplicationPool spsAppPool = sm.ApplicationPools.FirstOrDefault(s => s.Name.Equals(args.PublishingServiceSiteName));

        if (!DeleteSite(sm, spsSite))
        {
          throw new Exception($"Could not stop site with the name {args.PublishingServiceSiteName}.  Please remove it manually in IIS.");
        }

        if (!DeleteAppPool(sm, spsAppPool))
        {
          throw new Exception($"Could not stop app pool with the name {args.PublishingServiceSiteName}.  Please remove it manually in IIS.");
        }

        sm.CommitChanges();
      }

      if (Directory.Exists(args.PublishingServiceWebroot))
      {
        try
        {
          Directory.Delete(args.PublishingServiceWebroot, true);
        }
        catch (IOException ex)
        {
          //Unlikely user scenario, but it can occur when you create an instance, then immediately try to overwrite it without restarting SIM
          throw new Exception($"SIM may be locking the {args.PublishingServiceWebroot} folder if it was just created.  Try restarting SIM and installing publishing service again", ex);
        }
      }
    }

    private bool DeleteSite(ServerManager sm, [CanBeNull] Site site)
    {
      if (site != null)
      {
        int retries = 0;
        while (!site.State.Equals(ObjectState.Stopped) && retries < STOP_RETRIES)
        {
          //If the site is starting/stopping IIS won't accept commands and will throw an error, we'll check if its in a started state first
          if (site.State.Equals(ObjectState.Started))  
          {
            site.Stop();
          }
          retries++;
          Thread.Sleep(RETRY_INTERVAL_MS);
        }
        Log.Info($"The site {site.Name}'s state is currently {site.State}");
        if (!site.State.Equals(ObjectState.Stopped))
        {
          return false;
        }
        sm.Sites.Remove(site);
      }
      return true;
    }

    private bool DeleteAppPool (ServerManager sm, [CanBeNull] ApplicationPool appPool)
    {
      if (appPool != null)
      {
        int retries = 0;
        while (!appPool.State.Equals(ObjectState.Stopped) && retries < STOP_RETRIES)
        {
          //If the site is starting/stopping IIS won't accept commands and will throw an error, we'll check if its in a started state first
          if (appPool.State.Equals(ObjectState.Started))
          {
            appPool.Stop();
          }
          retries++;
          Thread.Sleep(RETRY_INTERVAL_MS);
        }
        Log.Info($"The appl pool {appPool.Name}'s state is currently {appPool.State}");
        if (!appPool.State.Equals(ObjectState.Stopped))
        {
          return false;
        }
        sm.ApplicationPools.Remove(appPool);
      }
      return true;
    }
  }
}

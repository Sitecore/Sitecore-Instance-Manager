using System;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Web.Administration;

namespace SIM.Pipelines.PublishingService.Uninstall
{
  public class RemoveIISSiteProcessor : SPSProcessor<UninstallSPSProcessorArgs>
  {
    protected override void ProcessCore([NotNull] UninstallSPSProcessorArgs args)
    {
      if (args.SkipSPSSite)
      {
        return;
      }

      using (ServerManager sm = new ServerManager())
      {
        Site spsSite = sm.Sites.FirstOrDefault(s => s.Name.Equals(args.SPSSiteName));

        if (!DeleteSite(sm, spsSite, args.MaxRetries, args.RetryInterval))
        {
          throw new Exception($"Could not stop site with the name {args.SPSSiteName}.  Please remove it manually in IIS.");
        }

        sm.CommitChanges();
      }
    }

    private bool DeleteSite(ServerManager sm, [CanBeNull] Site site, int maxRetries, int retryInterval)
    {
      if (site != null)
      {
        int retries = 0;
        while (!site.State.Equals(ObjectState.Stopped) && retries < maxRetries)
        {
          //If the site is starting/stopping IIS won't accept commands and will throw an error, we'll check if its in a started state first
          if (site.State.Equals(ObjectState.Started))
          {
            site.Stop();
          }
          retries++;
          Thread.Sleep(retryInterval);
        }
        if (!site.State.Equals(ObjectState.Stopped))
        {
          return false;
        }
        sm.Sites.Remove(site);
      }
      return true;
    }

  }
}

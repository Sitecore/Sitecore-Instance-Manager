using JetBrains.Annotations;
using Microsoft.Web.Administration;
using SIM.Pipelines.InstallPublishingService;
using System;
using System.Linq;
using System.Threading;

namespace SIM.Pipelines.UninstallPublishingService
{
  public class RemoveAppPoolProcessor : SPSProcessor<UninstallSPSProcessorArgs>
  {
    protected override void ProcessCore([NotNull] UninstallSPSProcessorArgs args)
    {
      if (args.SkipSPSAppPool)
      {
        return;
      }

      using (ServerManager sm = new ServerManager())
      {
        ApplicationPool spsAppPool = sm.ApplicationPools.FirstOrDefault(a => a.Name.Equals(args.SPSAppPoolName));

        if (!DeleteAppPool(sm, spsAppPool, args.MaxRetries, args.RetryInterval))
        {
          throw new Exception($"Could not stop application pool with the name {args.SPSAppPoolName}.  Please remove it manually in IIS.");
        }

        sm.CommitChanges();
      }
    }

    private bool DeleteAppPool(ServerManager sm, [CanBeNull] ApplicationPool appPool, int maxRetries, int retryInterval)
    {
      if (appPool != null)
      {
        int retries = 0;
        while (!appPool.State.Equals(ObjectState.Stopped) && retries < maxRetries)
        {
          //If the site is starting/stopping IIS won't accept commands and will throw an error, we'll check if its in a started state first
          if (appPool.State.Equals(ObjectState.Started))
          {
            appPool.Stop();
          }
          retries++;
          Thread.Sleep(retryInterval);
        }
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

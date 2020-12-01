using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SIM.Adapters.WebServer;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class AddHostsProcessor: Processor
  {
    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, nameof(arguments));

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      Assert.ArgumentNotNull(args, nameof(args));

      IEnumerable<string> hosts = GetHostNames(args);

      DoUpdate(hosts);
    }

    protected virtual void DoUpdate(IEnumerable<string> hosts)
    {
      foreach (var hostName in hosts)
      {
        Hosts.Append(hostName);
      }
    }

    protected virtual IEnumerable<string> GetHostNames(InstallContainerArgs args)
    {
      IEnumerable<string> hosts = args.EnvModel.Where(nvp => nvp.Name.EndsWith("_HOST")).Select(nvp => nvp.Value);

      return hosts;
    }
  }
}
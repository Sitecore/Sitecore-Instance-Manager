using System.Collections.Generic;
using JetBrains.Annotations;
using SIM.Adapters.WebServer;
using SIM.Pipelines.Install.Containers;

namespace SIM.Pipelines.Delete.Containers
{
  [UsedImplicitly]
  public class RemoveHostsProcessor : AddHostsProcessor
  {
    protected override void DoUpdate(IEnumerable<string> hosts)
    {
      Hosts.Remove(hosts);
    }
  }
}
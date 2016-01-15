namespace SIM.Core.Commands
{
  using System.Collections.Generic;
  using SIM.Core.Models;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListInstancesResult : Dictionary<string, InstanceInfo>
  {
    public ListInstancesResult([NotNull] IDictionary<string, InstanceInfo> instances)
      : base(instances)
    {
      Assert.ArgumentNotNull(instances, "instances");
    }
  }
}
namespace SIM.Core.Commands
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Models;

  public class ListCommandResult
  {
    private static readonly IReadOnlyDictionary<string, InstanceInfo> Empty = new ReadOnlyDictionary<string, InstanceInfo>(new Dictionary<string, InstanceInfo>());

    public ListCommandResult(IEnumerable<string> instances)
    {
      Assert.ArgumentNotNull(instances, "instances");

      this.Instances = instances.ToArray();
      this.Detailed = Empty;
    }

    public ListCommandResult(IEnumerable<InstanceInfo> instances)
    {
      Assert.ArgumentNotNull(instances, "instances");

      var detailed = instances.ToDictionary(x => x.Name, x => x);

      this.Detailed = detailed;
      this.Instances = detailed.Keys.ToArray();
    }

    /// <summary>
    /// Always contains the list of instance names.
    /// </summary>
    public IReadOnlyCollection<string> Instances { [UsedImplicitly] get; private set; }

    /// <summary>
    /// Can contain detailed instances information.
    /// </summary>
    public IReadOnlyDictionary<string, InstanceInfo> Detailed { [UsedImplicitly] get; private set; }
  }
}
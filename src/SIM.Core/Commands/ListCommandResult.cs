namespace SIM.Core.Commands
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core.Common;
  using SIM.Core.Models;

  public class ListCommandResult : CommandResult
  {
    private static readonly IReadOnlyDictionary<string, InstanceInfo> Empty = new ReadOnlyDictionary<string, InstanceInfo>(new Dictionary<string, InstanceInfo>());

    public ListCommandResult(IEnumerable<string> instances)
    {
      Assert.ArgumentNotNull(instances, nameof(instances));

      Instances = instances.ToArray();
      Detailed = Empty;
    }

    public ListCommandResult(IEnumerable<InstanceInfo> instances)
    {
      Assert.ArgumentNotNull(instances, nameof(instances));

      var detailed = instances.ToDictionary(x => x.Name, x => x);

      Detailed = detailed;
      Instances = detailed.Keys.ToArray();
    }

    /// <summary>
    ///   Always contains the list of instance names.
    /// </summary>
    public IReadOnlyCollection<string> Instances { [UsedImplicitly] get; private set; }

    /// <summary>
    ///   Can contain detailed instances information.
    /// </summary>
    public IReadOnlyDictionary<string, InstanceInfo> Detailed { [UsedImplicitly] get; private set; }
  }
}
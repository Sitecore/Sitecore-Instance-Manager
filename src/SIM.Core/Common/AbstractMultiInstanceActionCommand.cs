namespace SIM.Core.Common
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Instances;

  public abstract class AbstractMultiInstanceActionCommand<T> : AbstractCommand<T>
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult<T> result)
    {
      var instances = AbstractMultiInstanceActionCommand.GetInstances(Name);

      this.DoExecute(instances, result);
    }

    protected abstract void DoExecute(IReadOnlyList<Instance> instances, CommandResult<T> result);
  }

  public abstract class AbstractMultiInstanceActionCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult result)
    {
      var name = Name;
      var instance = GetInstances(name);

      this.DoExecute(instance, result);
    }

    [NotNull]
    internal static IReadOnlyList<Instance> GetInstances(string listString)
    {
      InstanceManager.Initialize();
      Assert.ArgumentNotNullOrEmpty(listString, nameof(listString));

      var names = listString.Split("|,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        .GroupBy(x => x).Select(x => x.Key) // DISTINCT
        .ToList();

      var result = new List<Instance>();
      var instances = InstanceManager.Instances.ToArray();
      foreach (var name in names.ToArray())
      {
        var instance = instances.FirstOrDefault(x => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
        if (instance != null)
        {
          result.Add(instance);
          names.Remove(name);
        }
      }

      if (!names.Any())
      {
        return result;
      }

      var ex = new InvalidOperationException($"Cannot find instances by name: {string.Join(", ", names)}");
      ex.Data.Add("names", names);

      throw ex;
    }

    protected abstract void DoExecute(IReadOnlyList<Instance> instances, CommandResult result);
  }
}
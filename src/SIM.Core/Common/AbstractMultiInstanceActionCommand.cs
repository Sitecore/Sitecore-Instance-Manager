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
    internal static IReadOnlyList<Instance> GetInstances(string name)
    {
      InstanceManager.Initialize();
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var names = name.Split("|,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      var instances = InstanceManager.Instances
        .Where(x => 
          names.Any(z => 
            string.Equals(z, x.Name, StringComparison.OrdinalIgnoreCase)))
        .ToArray();
                      
      return instances;
    }

    protected abstract void DoExecute(IReadOnlyList<Instance> instances, CommandResult result);
  }
}
namespace SIM.Core.Common
{
  using System;
  using System.Linq;
  using Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class AbstractInstanceActionCommand<T> : AbstractCommand<T>
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult<T> result)
    {
      InstanceManager.Initialize();
      var name = Name;
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, "instance is not found");
                                   
      this.DoExecute(instance, result);
    }

    protected abstract void DoExecute(Instance instance, CommandResult<T> result);
  }

  public abstract class AbstractInstanceActionCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult result)
    {
      InstanceManager.Initialize();
      var name = Name;
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, "instance is not found");

      this.DoExecute(instance, result);
    }

    protected abstract void DoExecute(Instance instance, CommandResult result);
  }
}
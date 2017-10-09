namespace SIM.Core.Common
{
  using System;
  using System.Linq;
  using Instances;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.IO;

  public abstract class AbstractInstanceActionCommand<T> : AbstractCommand<T>
  {
    protected AbstractInstanceActionCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }

    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult<T> result)
    {
      var instance = AbstractInstanceActionCommand.GetInstance(Name);
                                   
      DoExecute(instance, result);
    }

    protected abstract void DoExecute(Instance instance, CommandResult<T> result);
  }

  public abstract class AbstractInstanceActionCommand : AbstractCommand
  {
    protected AbstractInstanceActionCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
    }

    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected sealed override void DoExecute(CommandResult result)
    {
      var name = Name;
      var instance = GetInstance(name);

      DoExecute(instance, result);
    }

    [NotNull]
    internal static Instance GetInstance(string name)
    {
      InstanceManager.Default.Initialize();
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var instance = InstanceManager.Default.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, $"The {name} instance is not found");

      return instance;
    }

    protected abstract void DoExecute(Instance instance, CommandResult result);
  }
}
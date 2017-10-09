namespace SIM.Core.Commands
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.IO;

  public class StateCommand : AbstractInstanceActionCommand<string>
  {
    public StateCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }

    protected override void DoExecute(Instance instance, CommandResult<string> result)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(result, nameof(result));     

      result.Data = instance.State.ToString();
    }
  }
}
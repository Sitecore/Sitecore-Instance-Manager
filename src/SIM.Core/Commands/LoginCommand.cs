namespace SIM.Core.Commands
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.IO;

  public abstract class LoginCommand : AbstractInstanceActionCommand<Exception>
  {
    protected LoginCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }

    protected override void DoExecute(Instance instance, CommandResult<Exception> result)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(result, nameof(result));

      Ensure.IsTrue(instance.State != InstanceState.Disabled, "instance is disabled");
      Ensure.IsTrue(instance.State != InstanceState.Stopped, "instance is stopped");

      var url = CoreInstanceAuth.GenerateAuthUrl();
      var destFileName = CoreInstanceAuth.CreateAuthFile(instance, url);
      CoreInstance.Browse(instance, url);          
      WaitAndDelete(destFileName);
    }

    protected abstract void WaitAndDelete(string destFileName);
  }
}
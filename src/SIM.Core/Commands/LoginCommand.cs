namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public abstract class LoginCommand : AbstractInstanceActionCommand<Exception>
  {
    protected override void DoExecute(CommandResult<Exception> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      InstanceManager.Initialize();
      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, "instance is not found");
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
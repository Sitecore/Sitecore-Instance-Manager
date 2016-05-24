namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public abstract class LoginCommand : AbstractInstanceActionCommand<Exception>
  {
    protected override void DoExecute(CommandResultBase<Exception> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      InstanceManager.Initialize();
      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (instance == null)
      {
        result.Success = false;
        result.Message = "instance not found";

        return;
      }

      if (instance.State == InstanceState.Disabled)
      {
        result.Success = false;
        result.Message = "instance is disabled";

        return;
      }

      if (instance.State == InstanceState.Stopped)
      {
        result.Success = false;
        result.Message = "instance is stopped";

        return;
      }

      var url = CoreInstanceAuth.GenerateAuthUrl();
      var destFileName = CoreInstanceAuth.CreateAuthFile(instance, url);
      CoreInstance.Browse(instance, url);
      WaitAndDelete(destFileName);
      result.Success = true;
      result.Message = "done";
    }

    protected abstract void WaitAndDelete(string destFileName);
  }
}
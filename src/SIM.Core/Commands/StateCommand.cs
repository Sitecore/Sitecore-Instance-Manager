namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class StateCommand : AbstractInstanceActionCommand<string>
  {
    protected override void DoExecute(CommandResultBase<string> result)
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

      result.Success = true;
      result.Message = "done";
      result.Data = instance.State.ToString();
    }
  }
}
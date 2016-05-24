namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class BrowseCommand : AbstractInstanceActionCommand<Exception>
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
      
      CoreInstance.Browse(instance, "/sitecore");
    }
  }
}
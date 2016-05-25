namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class BrowseCommand : AbstractInstanceActionCommand<Exception>
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
      
      CoreInstance.Browse(instance, "/sitecore");
    }
  }
}
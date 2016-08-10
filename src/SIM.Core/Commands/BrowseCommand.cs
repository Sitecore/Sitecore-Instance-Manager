namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class BrowseCommand : AbstractInstanceActionCommand<Exception>
  {
    protected override void DoExecute(Instance instance, CommandResult<Exception> result)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(result, nameof(result));
                            
      Ensure.IsTrue(instance.State != InstanceState.Disabled, "instance is disabled");
      Ensure.IsTrue(instance.State != InstanceState.Stopped, "instance is stopped");

      CoreInstance.Browse(instance, "/sitecore");
    }
  }
}
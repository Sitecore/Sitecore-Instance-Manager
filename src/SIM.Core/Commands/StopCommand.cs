namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class StopCommand : AbstractInstanceActionCommand<Exception>
  {
    public virtual bool? Force { get; set; }

    protected override void DoExecute(CommandResult<Exception> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var force = this.Force; 
      var name = this.Name;
      Ensure.IsNotNullOrEmpty(name, "Name is not specified");
      
      InstanceManager.Initialize();
      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, "instance is not found");
      Ensure.IsTrue(instance.State != InstanceState.Disabled, "instance is disabled");
      Ensure.IsTrue(instance.State != InstanceState.Stopped, "instance is already stopped");

      instance.Stop(force);
    }
  }
}
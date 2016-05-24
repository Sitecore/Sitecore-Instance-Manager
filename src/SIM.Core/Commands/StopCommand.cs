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

    protected override void DoExecute(CommandResultBase<Exception> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var force = this.Force; 
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
        result.Message = "instance is already stopped";

        return;
      }

      Exception exception = null;
      try
      {
        instance.Stop(force);
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      result.Success = exception == null;
      result.Message = exception.With(x => x.Message) ?? "done";
      result.Data = exception;
    }
  }
}
namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using SIM.Core.Common;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;

  public class StartCommand : AbstractInstanceActionCommand<Exception>
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

      if (instance.State == InstanceState.Ready || instance.State == InstanceState.Running)
      {
        result.Success = false;
        result.Message = "instance is not stopped";

        return;
      }

      Exception exception = null;
      try
      {
        instance.Start();
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
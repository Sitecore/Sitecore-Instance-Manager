namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.Delete;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class DeleteCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    protected override void DoExecute(CommandResultBase result)
    {
      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var profile = Profile.Read();
      var connectionString = profile.GetValidConnectionString();

      InstanceManager.Initialize();
      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (instance == null)
      {
        result.Success = false;
        result.Message = "instance not found";

        return;
      }

      PipelineManager.Initialize();

      var deleteArgs = new DeleteArgs(instance, connectionString);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("delete", deleteArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages();
    }
  }
}
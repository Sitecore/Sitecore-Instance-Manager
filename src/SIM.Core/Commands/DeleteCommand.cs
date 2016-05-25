namespace SIM.Core.Commands
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.Delete;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class DeleteCommand : AbstractInstanceActionCommand<string[]>
  {
    protected override void DoExecute(CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var profile = Profile.Read();
      var connectionString = profile.GetValidConnectionString();

      InstanceManager.Initialize();
      var instance = InstanceManager.Instances.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      Ensure.IsNotNull(instance, "instance is not found");

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);

      var deleteArgs = new DeleteArgs(instance, connectionString);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("delete", deleteArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }
  }
}
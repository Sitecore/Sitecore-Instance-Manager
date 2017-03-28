namespace SIM.Core.Commands
{
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.Delete;

  public class DeleteCommand : AbstractInstanceActionCommand<string[]>
  {
    protected override void DoExecute(Instance instance, CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(result, nameof(result));             

      var profile = Profile.Read();
      var connectionString = profile.GetValidConnectionString();    

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
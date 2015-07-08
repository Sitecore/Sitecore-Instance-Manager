namespace SIM.Pipelines.MultipleDeletion
{
  using System.Linq;
  using Instances;
  using Delete;
  using Processors;
  using Base;
  
  public class MultipleDeletion : MultipleDeletionProcessor
  {
    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      return ((MultipleDeletionArgs)args).Instances.Count;
    }

    protected override void Process(MultipleDeletionArgs args)
    {
      foreach (var deleteArgs in args.Instances.Select(InstanceManager.GetInstance).NotNull().Select(instance => new DeleteArgs(instance, args.ConnectionString)))
      {
        PipelineManager.StartPipeline("delete", deleteArgs, null, false);
        IncrementProgress();
      }
    }
  }
}

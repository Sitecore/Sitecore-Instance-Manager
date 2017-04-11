namespace SIM.Pipelines.MultipleDeletion
{
  using System.Linq;
  using SIM.Extensions;
  using SIM.Instances;
  using SIM.Pipelines.Delete;
  using SIM.Pipelines.Processors;

  public class MultipleDeletion : MultipleDeletionProcessor
  {
    #region Public methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      return ((MultipleDeletionArgs)args).Instances.Count;
    }

    #endregion

    #region Protected methods

    protected override void Process(MultipleDeletionArgs args)
    {
      foreach (var deleteArgs in args.Instances.Select(InstanceManager.Default.GetInstance).NotNull().Select(instance => new DeleteArgs(instance, args._ConnectionString)))
      {
        PipelineManager.StartPipeline("delete", deleteArgs, null, false);
        this.IncrementProgress();
      }
    }

    #endregion
  }
}
namespace SIM.Pipelines.Processors
{
  using System.Collections.Generic;

  public class MultipleProcessorDefinition : ProcessorDefinition
  {
    #region Public Methods and Operators

    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args, IPipelineController controller)
    {
      var hive = (ProcessorHive)ReflectionUtil.CreateObject(Type,this);
      IEnumerable<Processor> proc=hive.CreateProcessors(args);
      if (controller != null)
      {
        this.SetControllerForDynamicNestedProcessors(proc, controller);
      }

      return proc;
    }

    private void SetControllerForDynamicNestedProcessors(IEnumerable<Processor>processors, IPipelineController controller)
    {
      foreach(Processor p in processors)
      {
        p.Controller = controller;
        if (p._NestedProcessors != null && p._NestedProcessors.Count > 0)
        {
          this.SetControllerForDynamicNestedProcessors(p._NestedProcessors, controller);
        }
      }
    }

    #endregion
  }
}
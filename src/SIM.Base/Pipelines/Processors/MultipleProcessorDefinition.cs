namespace SIM.Pipelines.Processors
{
  using System.Collections.Generic;

  public class MultipleProcessorDefinition : ProcessorDefinition
  {
    #region Public Methods and Operators

    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      var hive = (ProcessorHive)ReflectionUtil.CreateObject(this.Type);
      return hive.CreateProcessors(args);
    }

    #endregion
  }
}
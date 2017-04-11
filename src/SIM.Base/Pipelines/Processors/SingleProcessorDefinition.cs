namespace SIM.Pipelines.Processors
{
  using System.Collections.Generic;

  public class SingleProcessorDefinition : ProcessorDefinition
  {
    #region Public Methods and Operators

    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      return new[]
      {
        (Processor)ReflectionUtil.CreateObject(Type)
      };
    }

    #endregion
  }
}
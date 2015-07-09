namespace SIM.Pipelines.Processors
{
  using System.Collections.Generic;

  public abstract class ProcessorHive
  {
    #region Public Methods and Operators

    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args);

    #endregion
  }
}
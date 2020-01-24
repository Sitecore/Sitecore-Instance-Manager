using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Processors
{
  using System.Collections.Generic;

  public abstract class ProcessorHive
  {
    protected ProcessorDefinition ParentDefinition { get; set; }
    #region Public Methods and Operators

    public ProcessorHive(ProcessorDefinition parentDefinition)
    {
      Assert.ArgumentNotNull(parentDefinition, nameof(parentDefinition));
      this.ParentDefinition = parentDefinition;
    }
    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args);

    #endregion
  }
}
namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  #region

  #endregion

  public abstract class ProcessorDefinition
  {
    #region Fields

    [NotNull]
    public readonly List<ProcessorDefinition> _NestedProcessorDefinitions = new List<ProcessorDefinition>();

    #endregion

    #region Public Properties

    public string OwnerPipelineName { get; set; }

    [CanBeNull]
    public string Param { get; set; }

    public bool ProcessAlways { get; set; }

    [NotNull]
    public string Title { get; set; }

    [NotNull]
    public Type Type { get; set; }

    #endregion

    #region Public Methods and Operators

    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args, IPipelineController controller);

    public override string ToString()
    {
      return Title.Replace("{param}", Param);
    }

    #endregion
  }
}
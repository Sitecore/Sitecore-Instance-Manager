namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public abstract class ProcessorDefinition
  {
    #region Fields

    [NotNull]
    public readonly List<ProcessorDefinition> NestedProcessorDefinitions = new List<ProcessorDefinition>();

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

    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args);

    public override string ToString()
    {
      return this.Title.Replace("{param}", this.Param);
    }

    #endregion
  }
}
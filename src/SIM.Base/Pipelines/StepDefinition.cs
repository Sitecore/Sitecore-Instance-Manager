namespace SIM.Pipelines
{
  #region

  using System.Collections.Generic;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #endregion

  public class StepDefinition
  {
    #region Fields

    [NotNull]
    public readonly List<ProcessorDefinition> _ProcessorDefinitions;

    #endregion

    #region Constructors

    public StepDefinition([NotNull] List<ProcessorDefinition> processorDefinitions, [CanBeNull] string argsName = null)
    {
      Assert.ArgumentNotNull(processorDefinitions, nameof(processorDefinitions));

      this.ArgsName = argsName;
      this._ProcessorDefinitions = processorDefinitions;
    }

    #endregion

    #region Properties

    [CanBeNull]
    public string ArgsName { get; set; }

    #endregion
  }
}
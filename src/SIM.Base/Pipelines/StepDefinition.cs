namespace SIM.Pipelines
{
  #region

  using System.Collections.Generic;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class StepDefinition
  {
    #region Fields

    [NotNull]
    public readonly List<ProcessorDefinition> ProcessorDefinitions;

    #endregion

    #region Constructors

    public StepDefinition([NotNull] List<ProcessorDefinition> processorDefinitions, [CanBeNull] string argsName = null)
    {
      Assert.ArgumentNotNull(processorDefinitions, "processorDefinitions");

      this.ArgsName = argsName;
      this.ProcessorDefinitions = processorDefinitions;
    }

    #endregion

    #region Properties

    [CanBeNull]
    public string ArgsName { get; set; }

    #endregion
  }
}
#region Usings

using System.Collections.Generic;
using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   The step definition.
  /// </summary>
  public class StepDefinition
  {
    #region Fields

    /// <summary>
    ///   The processor definitions.
    /// </summary>
    [NotNull]
    public readonly List<ProcessorDefinition> ProcessorDefinitions;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StepDefinition"/> class.
    /// </summary>
    /// <param name="processorDefinitions">
    /// The processor definitions. 
    /// </param>
    /// <param name="argsName">
    /// The args name. 
    /// </param>
    public StepDefinition([NotNull] List<ProcessorDefinition> processorDefinitions, [CanBeNull] string argsName = null)
    {
      Assert.ArgumentNotNull(processorDefinitions, "processorDefinitions");

      this.ArgsName = argsName;
      this.ProcessorDefinitions = processorDefinitions;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets ArgsName.
    /// </summary>
    [CanBeNull]
    public string ArgsName { get; set; }

    #endregion
  }
}
#region Usings

using System;
using System.Collections.Generic;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Processors
{

  #region

  #endregion

  /// <summary>
  ///   The processor definition.
  /// </summary>
  public abstract class ProcessorDefinition
  {
    #region Fields

    /// <summary>
    ///   The nested processor definitions.
    /// </summary>
    [NotNull] public readonly List<ProcessorDefinition> NestedProcessorDefinitions = new List<ProcessorDefinition>();

    #endregion

    #region Public Properties

    public string OwnerPipelineName { get; set; }

    /// <summary>
    ///   Gets or sets Param.
    /// </summary>
    [CanBeNull]
    public string Param { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ProcessAlways.
    /// </summary>
    public bool ProcessAlways { get; set; }

    /// <summary>
    ///   Gets or sets Title.
    /// </summary>
    [NotNull]
    public string Title { get; set; }

    /// <summary>
    ///   Gets or sets Type.
    /// </summary>
    [NotNull]
    public Type Type { get; set; }

    #endregion

    #region Public Methods and Operators

    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args);

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The to string. </returns>
    public override string ToString()
    {
      return this.Title.Replace("{param}", this.Param);
    }

    #endregion
  }
}
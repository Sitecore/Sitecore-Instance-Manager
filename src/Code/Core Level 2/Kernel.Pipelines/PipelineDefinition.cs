#region Usings

using System.Collections.Generic;

#endregion

namespace SIM.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   The pipeline definition.
  /// </summary>
  public class PipelineDefinition
  {
    #region Properties

    /// <summary>
    ///   Gets or sets Steps.
    /// </summary>
    public List<StepDefinition> Steps { get; set; }

    /// <summary>
    ///   Gets or sets Title.
    /// </summary>
    public string Title { get; set; }

    #endregion
  }
}
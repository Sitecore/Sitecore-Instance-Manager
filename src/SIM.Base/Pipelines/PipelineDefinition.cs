namespace SIM.Pipelines
{
  using System.Collections.Generic;

  #region

  #endregion

  public class PipelineDefinition
  {
    #region Properties

    public List<StepDefinition> Steps { get; set; }

    public string Title { get; set; }

    #endregion
  }
}
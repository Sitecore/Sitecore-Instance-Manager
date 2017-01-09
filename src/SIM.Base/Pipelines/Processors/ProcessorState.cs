namespace SIM.Pipelines.Processors
{
  #region

  using JetBrains.Annotations;

  #endregion

  public enum ProcessorState
  {
    /// <summary>
    ///   The waiting.
    /// </summary>
    [UsedImplicitly]
    Waiting, 

    Running, 

    Done, 

    Error, 

    Inaccessible
  }
}
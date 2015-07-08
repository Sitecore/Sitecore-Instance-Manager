#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Processors
{
  #region

  

  #endregion

  /// <summary>
  ///   The processor state.
  /// </summary>
  public enum ProcessorState
  {
    /// <summary>
    ///   The waiting.
    /// </summary>
    [UsedImplicitly]
    Waiting, 

    /// <summary>
    ///   The running.
    /// </summary>
    Running, 

    /// <summary>
    ///   The done.
    /// </summary>
    Done, 

    /// <summary>
    ///   The error.
    /// </summary>
    Error, 

    /// <summary>
    ///   The inaccessible.
    /// </summary>
    Inaccessible
  }
}
namespace SIM.Adapters.WebServer
{
  /// <summary>
  ///   The app pool info.
  /// </summary>
  public class AppPoolInfo
  {
    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether Enable32BitAppOnWin64.
    /// </summary>
    public bool Enable32BitAppOnWin64 { get; set; }

    /// <summary>
    ///   Gets or sets FrameworkVersion.
    /// </summary>
    public string FrameworkVersion { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ManagedPipelineMode.
    /// </summary>
    public bool ManagedPipelineMode { get; set; }

    #endregion
  }
}
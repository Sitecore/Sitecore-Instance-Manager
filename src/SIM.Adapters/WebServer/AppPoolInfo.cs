namespace SIM.Adapters.WebServer
{
  /// <summary>
  ///   The app pool info.
  /// </summary>
  public class AppPoolInfo
  {
    #region Properties

    public bool Enable32BitAppOnWin64 { get; set; }

    public string FrameworkVersion { get; set; }

    public bool ManagedPipelineMode { get; set; }

    #endregion
  }
}
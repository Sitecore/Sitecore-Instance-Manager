using SIM.Instances;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.UninstallPublishingService
{
  public class UninstallSPSProcessorArgs : ProcessorArgs
  {
    #region Properties
    public Instance Instance { get; set; }
    public string SPSSiteName { get; set; }
    public string SPSAppPoolName { get; set; }
    public string SPSWebroot { get; set; }
    public bool SkipSPSSite { get; set; }
    public bool SkipSPSAppPool { get; set; }
    public bool SkipSPSWebroot { get; set; }
    public int MaxRetries { get; set; } = 6;
    public int RetryInterval { get; set; } = 5000;

    #endregion
  }
}

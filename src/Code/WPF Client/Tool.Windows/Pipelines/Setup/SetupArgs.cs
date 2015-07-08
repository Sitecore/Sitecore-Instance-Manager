using SIM.Pipelines.Processors;

namespace SIM.Tool.Windows.Pipelines.Setup
{
  public class SetupArgs : ProcessorArgs
  {
    public string InstancesRootFolderPath { get; set; }

    public string LocalRepositoryFolderPath { get; set; }

    public string LicenseFilePath { get; set; }

    public string ConnectionString { get; set; }
  }
}

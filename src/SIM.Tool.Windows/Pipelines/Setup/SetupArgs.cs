namespace SIM.Tool.Windows.Pipelines.Setup
{
  using SIM.Pipelines.Processors;

  public class SetupArgs : ProcessorArgs
  {
    #region Public properties

    public string ConnectionString { get; set; }
    public string InstancesRootFolderPath { get; set; }

    public string LicenseFilePath { get; set; }
    public string LocalRepositoryFolderPath { get; set; }

    #endregion
  }
}
using SIM.Base;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.Pipelines.Setup
{
  public class SetupWizardArgs : WizardArgs
  {
    public SetupWizardArgs()
    {
    }

    public SetupWizardArgs(Profile profile)
    {
      if (profile == null)
      {
        return;
      }

      this.InstancesRootFolderPath = profile.InstancesFolder;
      this.LicenseFilePath = profile.License;
      this.ConnectionString = profile.ConnectionString;
      this.LocalRepositoryFolderPath = profile.LocalRepository;
    }

    public string InstancesRootFolderPath { get; set; }

    public string LocalRepositoryFolderPath { get; set; }

    public string LicenseFilePath { get; set; }

    public string ConnectionString { get; set; }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new SetupArgs() { InstancesRootFolderPath = this.InstancesRootFolderPath, LocalRepositoryFolderPath = this.LocalRepositoryFolderPath, LicenseFilePath = this.LicenseFilePath, ConnectionString = this.ConnectionString };
    }
  }
}

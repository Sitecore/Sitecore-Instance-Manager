namespace SIM.Tool.Windows.Pipelines.Setup
{
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using System;

  public class SetupWizardArgs : WizardArgs
  {
    #region Constructors

    public SetupWizardArgs()
    {
    }

    public SetupWizardArgs(Profile profile)
    {
      if (profile == null)
      {
        return;
      }

      InstancesRootFolderPath = profile.InstancesFolder;
      LicenseFilePath = profile.License;
      ConnectionString = profile.ConnectionString;
      LocalRepositoryFolderPath = profile.LocalRepository;
    }

    #endregion

    #region Public properties

    public string ConnectionString { get; set; }

    public string InstancesRootFolderPath { get; set; }

    public string LicenseFilePath { get; set; }
    public string LocalRepositoryFolderPath { get; set; }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      return new ProcessorArgs();
    }

    #endregion
  }
}

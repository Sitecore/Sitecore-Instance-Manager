namespace SIM.Tool.Windows.Pipelines.Setup
{
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class SetupProcessor : SaveAgreement, IAfterLastWizardPipelineStep
  {
    public new void Execute(WizardArgs wargs)
    {
      base.Execute(wargs);

      var args = (SetupWizardArgs)wargs;
      var profile = ProfileManager.Profile ?? new Profile();
      profile.ConnectionString = args.ConnectionString;
      profile.InstancesFolder = args.InstancesRootFolderPath;
      profile.License = args.LicenseFilePath;
      profile.LocalRepository = args.LocalRepositoryFolderPath;
      ProfileManager.SaveChanges(profile);
    }
  }
}
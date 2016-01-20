namespace SIM.Tool.Windows.Pipelines.Setup
{
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;

  public class SetupProcessor : Processor
  {
    #region Protected methods

    protected override void Process(ProcessorArgs processorArgs)
    {
      var args = (SetupArgs)processorArgs;
      var profile = ProfileManager.Profile ?? new Profile();
      profile.ConnectionString = args.ConnectionString;
      profile.InstancesFolder = args.InstancesRootFolderPath;
      profile.License = args.LicenseFilePath;
      profile.LocalRepository = args.LocalRepositoryFolderPath;
      ProfileManager.SaveChanges(profile);
    }

    #endregion
  }
}
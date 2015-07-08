using SIM.Pipelines.Processors;
using SIM.Tool.Base.Profiles;

namespace SIM.Tool.Windows.Pipelines.Setup
{
  public class SetupProcessor : Processor
  {
    protected override void Process(ProcessorArgs processorArgs)
    {
      var args = (SetupArgs)processorArgs;
      var profile = ProfileManager.Profile ?? new Profile();
      profile.ConnectionString = args.ConnectionString;
      profile.InstancesFolder = args.InstancesRootFolderPath;
      profile.License = args.LicenseFilePath;
      profile.LocalRepository = args.LocalRepositoryFolderPath;
      profile.Plugins = string.Empty;
      ProfileManager.SaveChanges(profile);
    }
  }
}

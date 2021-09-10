using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public static class LogFileFolderFactory
  {
    public static LogFileFolderResolver GetResolver(Instance instance)
    {
      if (instance.Type == Instance.InstanceType.SitecoreMember)
      {
        return new SitecoreMembersLogFileFolderResolver(instance);
      }

      return new SitecoreDefaultLogFileFolderResolver(instance);
    }
  }
}
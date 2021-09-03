using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public class LogFileFolderResolver
  {
    private readonly Instance _instance;

    public LogFileFolderResolver(Instance instance)
    {
      this._instance = instance;
    }

    public string GetLogFolder()
    {
      if (_instance.Type == Instance.InstanceType.SitecoreMember)
      {
        return new SitecoreMembersLogFileFolderFactory().GetLogFolder(_instance);
      }

      return new SitecoreDefaultLogFileFolderFactory().GetLogFolder(_instance);
    }
  }
}
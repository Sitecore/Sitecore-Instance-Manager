using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public class SitecoreMembersLogFileFolderResolver : LogFileFolderResolver
  {
    private readonly Instance _instance;

    public SitecoreMembersLogFileFolderResolver(Instance instance)
    {
      this._instance = instance;
    }

    public override string GetLogFolder()
    {
      // Sitecore members like Identity Server contain the logs folder inside the root folder
      string logsPath = FileSystem.FileSystem.Local.Directory.MapPath("logs", _instance.WebRootPath);
      if (FileSystem.FileSystem.Local.Directory.Exists(logsPath))
      {
        return logsPath;
      }

      return null;
    }
  }
}
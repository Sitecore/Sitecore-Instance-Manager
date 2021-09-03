using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public class SitecoreMembersLogFileFolderFactory : LogFileFolderFactory
  {
    public override string GetLogFolder(Instance instance)
    {
      // Sitecore members like Identity Server contain the logs folder inside the root folder
      string logsPath = FileSystem.FileSystem.Local.Directory.MapPath("logs", instance.WebRootPath);
      if (FileSystem.FileSystem.Local.Directory.Exists(logsPath))
      {
        return logsPath;
      }

      return new SitecoreDefaultLogFileFolderFactory().GetLogFolder(instance);
    }
  }
}
using System.IO;
using SIM.Extensions;
using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public class SitecoreDefaultLogFileFolderFactory : LogFileFolderFactory
  {
    public override string GetLogFolder(Instance instance)
    {
      var dataFolderPath = instance.DataFolderPath;

      FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));

      return Path.Combine(dataFolderPath, "logs");
    }
  }
}
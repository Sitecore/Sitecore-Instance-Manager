using System.IO;
using SIM.Extensions;
using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public class SitecoreDefaultLogFileFolderResolver : LogFileFolderResolver
  {
    private readonly Instance _instance;

    public SitecoreDefaultLogFileFolderResolver(Instance instance)
    {
      this._instance = instance;
    }

    public override string GetLogFolder()
    {
      var dataFolderPath = _instance.DataFolderPath;

      FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, _instance.Name));

      return Path.Combine(dataFolderPath, "logs");
    }
  }
}
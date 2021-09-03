using SIM.Instances;

namespace SIM.Tool.Windows.LogFileFolder
{
  public abstract class LogFileFolderFactory
  {
    public abstract string GetLogFolder(Instance instance);
  }
}
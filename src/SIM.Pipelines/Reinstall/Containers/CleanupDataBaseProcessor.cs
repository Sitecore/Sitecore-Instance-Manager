using System.IO;
using JetBrains.Annotations;
using SIM.Loggers;
using SIM.Pipelines.Delete.Containers;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Reinstall.Containers
{
  [UsedImplicitly]
  public abstract class CleanupDataBaseProcessor : Processor
  {
    protected abstract string DataFolder { get; }

    private ILogger _logger;

    protected override void Process(ProcessorArgs procArgs)
    {
      DeleteContainersArgs args = (DeleteContainersArgs)procArgs;

      Assert.ArgumentNotNull(args, "args");

      this._logger = args.Logger;

      string dataFolder = Path.Combine(args.DestinationFolder, DataFolder);

      CleanupFolder(dataFolder);
    }

    private void CleanupFolder(string dataFolder)
    {
      if (!Directory.Exists(dataFolder))
      {
        this._logger.Warn($"The '{dataFolder}' folder does not exist.");

        return;
      }

      DirectoryInfo parentDirectoryInfo = new DirectoryInfo(dataFolder);

      FileInfo[] fileInfos = parentDirectoryInfo.GetFiles();
      DirectoryInfo[] directoryInfos = parentDirectoryInfo.GetDirectories();

      this._logger.Info($"Deleting file(s) and folder(s) from the '{dataFolder}' folder");

      foreach (FileInfo fileInfo in fileInfos)
      {
        fileInfo.Delete();
      }

      foreach (DirectoryInfo directoryInfo in directoryInfos)
      {
        directoryInfo.Delete(recursive: true);
      }

      this._logger.Info($"The '{dataFolder}' folder has been clean up.");
    }
  }
}